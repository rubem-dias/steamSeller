using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using SteamItemSeller.Application.Dto;
using SteamItemSeller.Services.Dtos;
using SteamItemSeller.Services.SteamServices.Interfaces;
using SteamItemSeller.Util;
using EnumExtensions = SteamItemSeller.Util.EnumExtensions;

namespace SteamItemSeller.Services.ApiServices;
public class UserInventory : IUserInventory
{
    private const string BaseUri = "https://steamcommunity.com/inventory";
    private readonly HttpClient httpClient;
    private readonly HttpRetryHandler httpRetryHandler;
    private const string SteamAppId = "753";
    private const string ContextId = "6";

    public UserInventory(HttpClient httpClient, HttpRetryHandler httpRetryHandler)
    {
        this.httpClient = httpClient;
        this.httpRetryHandler = httpRetryHandler;
    }

    public async Task<List<ItemPostOrder?>> OrderedItemsToSell(UserData userData, InputFilter filter, string sessionId)
    {
        try
        {
            var credentials = await GetUserInventoryCredentials(userData);
            var completeInventoryUrl = $"{BaseUri}/{credentials.SteamId}/{credentials.AppId}/{credentials.ContextId}";

            var handleInventoryResponse = await HandleInventoryResponse(completeInventoryUrl, filter);
            var handleItemsObjectToSell = await HandleItemsObjectsToSell(handleInventoryResponse!, credentials!, sessionId);

            var itemOrdersResponse = await PostItemOrders(handleItemsObjectToSell, userData);

            return itemOrdersResponse!;
        }
        catch (Exception)
        {
            throw new Exception("There are no items eligible to be sold");
        }
    }
    private async Task<UserWalletCredentials> GetUserInventoryCredentials(UserData userData)
    {
        var walletCredentials = new UserWalletCredentials();

        #region Get Infos From Profile Url


        httpClient.DefaultRequestHeaders.Remove("Cookie");
        httpClient.DefaultRequestHeaders.Add("Cookie", userData.Cookie);

        var document = await httpClient
            .GetAsync($"{userData.ProfileUrl}inventory").Result.Content
            .ReadAsStringAsync();

        walletCredentials.SteamId = Regex.Match(document, @"g_steamID = ""(\d+)""").Groups[1].Value;
        walletCredentials.AppId = SteamAppId;
        walletCredentials.ContextId = ContextId;

        #endregion

        #region Get Infos From Profile Inventory Url

        document = await httpClient
                .GetAsync($"{userData.ProfileUrl}/inventory").Result.Content
                .ReadAsStringAsync();

        walletCredentials.Currency = Regex.Match(document, @"""wallet_currency"":(\d+)").Groups[1].Value;
        walletCredentials.Country = Regex.Match(document, @"\""wallet_country\"":\""(.*?)\""").Groups[1].Value;

        #endregion

        return walletCredentials;

    }
    private async Task<InventoryResponse?> HandleInventoryResponse(string completeInventoryUrl, InputFilter filter)
    {
        HttpResponseMessage response = await httpRetryHandler.ExecuteAsync(() => httpClient.GetAsync(completeInventoryUrl));

        if (!response.IsSuccessStatusCode)
            throw new Exception("Unable to fetch items");

        var responseContent = await response.Content.ReadAsStringAsync();
        var inventory = JsonConvert.DeserializeObject<InventoryResponse>(responseContent);

        if (inventory != null)
        {
            return await InventoryFilter(inventory, filter);
        }

        return inventory;
    }
    private static Task<InventoryResponse> InventoryFilter(InventoryResponse inventory, InputFilter filter)
    {
        var inventoryFiltered = new InventoryResponse();
        
        var currentCategory = EnumExtensions.GetDisplayName(filter.Category)
            .ToLower()
            .Replace(" ", "");

        var filteredDescriptions = inventory.Descriptions
            .Where(description => description.Tags
                .Any(tag => tag.Category!.ToLower() == currentCategory || tag.InternalName!.ToLower() == currentCategory && description.Tradable == 1)
            ).ToList();
        
        var filteredAssetsByDescriptions = inventory.Assets
            .Where(asset => filteredDescriptions
                 .Any(description => description.ClassId == asset.ClassId)
            ).ToList();

        inventoryFiltered.Assets = filteredAssetsByDescriptions;
        inventoryFiltered.Descriptions = filteredDescriptions;
        
        return Task.FromResult(inventoryFiltered);
    }
    private async Task<List<ItemToSell>> HandleItemsObjectsToSell(InventoryResponse inventory, UserWalletCredentials walletCredentials, string sessionId)
    {
        var itemsToSell = new List<ItemToSell>();

        try
        {
            foreach (var item in inventory.Descriptions)
            {

                var itemAsset = inventory.Assets.Where(asset => asset.ClassId == item.ClassId).FirstOrDefault();

                if (itemAsset == null)
                    continue;

                var itemPrice = await HandleItemsOverviewPrices(item, walletCredentials);
                var itemPriceConverted = itemPrice.MedianPrice != null ? itemPrice.MedianPrice.ConvertPrice() : itemPrice.LowestPrice!.ConvertPrice();

                if (itemPriceConverted == null)
                    continue;

                var itemToSell = new ItemToSell
                {
                    SessionId = sessionId,
                    AppId = walletCredentials.AppId,
                    ContextId = walletCredentials.ContextId,
                    AssetId = itemAsset!.AssetId!,
                    Amount = itemAsset!.Amount!,
                    Price = itemPriceConverted,
                    Name = item.Name
                };

                itemsToSell.Add(itemToSell);
            }
        } catch (Exception ex)
        {
            throw;
        }

        return itemsToSell;
    }
    private async Task<ItemPriceOverviewResponse> HandleItemsOverviewPrices(Description itemDescription, UserWalletCredentials userWalletCredentials)
    {
        try
        {
            var marketUrl = $"https://steamcommunity.com/market/priceoverview/?country={userWalletCredentials.Country}" +
                $"&currency={userWalletCredentials.Currency}" +
                $"&appid={userWalletCredentials.AppId}" +
                $"&market_hash_name={itemDescription.MarketHashName.Replace("&", "%26")}";

            HttpResponseMessage response = await httpRetryHandler.ExecuteAsync(() => httpClient.GetAsync(marketUrl));
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve item prices");

            var responseContent = await response.Content.ReadAsStringAsync();
            var itemPriceOverview = JsonConvert.DeserializeObject<ItemPriceOverviewResponse>(responseContent);


            return itemPriceOverview!;
        } catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
    private async Task<List<ItemPostOrder>> PostItemOrders(List<ItemToSell> items, UserData userData)
    {
        List<ItemPostOrder> itemsOrders = new List<ItemPostOrder>();
        var sellItemMarketUrl = "https://steamcommunity.com/market/sellitem/";

        httpClient.DefaultRequestHeaders.Remove("Cookie");
        httpClient.DefaultRequestHeaders.Add("Cookie", userData.Cookie.Replace("sessionId", "sessionid"));

        httpClient.DefaultRequestHeaders.Referrer = new Uri($"{userData.ProfileUrl}inventory");

        foreach (var item in items)
        {
            var formData = new Dictionary<string, string>
            {
                { "sessionid", item.SessionId},
                { "appid", item.AppId.ToString()},
                { "contextid", item.ContextId},
                { "assetid", item.AssetId},
                { "amount", item.Amount.ToString()},
                { "price", item.Price.ToString()}
            };

            var content = new FormUrlEncodedContent(formData);

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

            HttpResponseMessage response = await httpRetryHandler.ExecuteAsync(() => httpClient.PostAsync(sellItemMarketUrl, content));

            var itemOrder = new ItemPostOrder
            {
                Name = item.Name!,
                Price = item.Price,
                CreatedAt = DateTime.Now,
                Status = response.IsSuccessStatusCode ? Status.Ok : Status.Failed
            };

            itemsOrders.Add(itemOrder);
        }

        return itemsOrders;
    }
}