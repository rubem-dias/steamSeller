using System.Text.RegularExpressions;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using SteamItemSeller.Application.Dto;
using SteamItemSeller.Application.Exceptions;
using SteamItemSeller.Services.Dtos;
using SteamItemSeller.Services.SteamServices.Interfaces;
using EnumExtensions = SteamItemSeller.Util.EnumExtensions;

namespace SteamItemSeller.Services.ApiServices;
public class UserInventory(HttpClient httpClient) : IUserInventory
{
    private string _document = string.Empty;
    private const string BaseUri = "https://steamcommunity.com/inventory";
    public async Task<InventoryDto?> GetAllItems(string profileUri, InputFilter filter)
    {
        try
        {
            var getInventoryCredentials = await GetUserInventoryCredentials(profileUri);
            var completeInventoryUrl = $"{BaseUri}/{getInventoryCredentials[0]}/{getInventoryCredentials[1]}/{getInventoryCredentials[2]}";
            
            var handleInventoryResponse = await HandleInventoryResponse(completeInventoryUrl, filter);
            
            return handleInventoryResponse;
            
        } catch (UserInventoryException)
        {
            throw new UserInventoryException("There are no items eligible to be sold");
        }
    }
    private async Task<List<string>> GetUserInventoryCredentials(string profileUri)
    {
        var completeCredentials = new List<string>();
        
        _document = await httpClient
            .GetAsync(profileUri).Result.Content
            .ReadAsStringAsync();
        
        var inventoryCredentials = Regex.Match(_document, @"https://steamcommunity.com/id/.+/inventory/#([^""]+)")
            .Groups[1]
            .Value;
        
        var profileId = Regex.Match(_document, @"""steamid"":""(\d+)""")
            .Groups[1]
            .Value;

        var inventoryCredentialsToList = inventoryCredentials
            .Split("_");
        
        completeCredentials.Add(profileId);
        completeCredentials.Add(inventoryCredentialsToList[0]);
        completeCredentials.Add(inventoryCredentialsToList[1]);

        return completeCredentials;
    }

    private async Task<InventoryDto?> HandleInventoryResponse(string completeInventoryUrl, InputFilter filter)
    {
        var response = await httpClient.GetAsync(completeInventoryUrl);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("Unable to fetch items");
        
        var inventory = JsonConvert.DeserializeObject<InventoryDto>(responseContent);
        
        if (filter.Category != Category.None && inventory != null)
            inventory = await InventoryFilter(inventory!, filter);
        
        return inventory;
    }

    private static Task<InventoryDto> InventoryFilter(InventoryDto inventory, InputFilter filter)
    {
        var inventoryFiltered = new InventoryDto();
        
        var currentCategory = EnumExtensions.GetDisplayName(filter.Category)
            .ToLower()
            .Replace(" ", "");

        var filteredDescriptions = inventory.Descriptions
            .Where(description => description.Tags
                .Any(tag => tag.Category == currentCategory)
            ).ToList();
        
        var filteredAssetsByDescriptions = inventory.Assets.Where(asset =>
            filteredDescriptions.Any(description => description.ClassId == asset.ClassId)
        ).ToList();

        inventoryFiltered.Assets = filteredAssetsByDescriptions;
        inventoryFiltered.Descriptions = filteredDescriptions;
        
        return Task.FromResult(inventoryFiltered);
    }
}