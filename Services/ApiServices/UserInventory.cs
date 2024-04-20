using SteamItemSeller.Services.SteamServices.Interfaces;

namespace SteamItemSeller.Services.SteamServices;
public class UserInventory : IUserInventory
{
    private readonly HttpClient _httpClient;
    public UserInventory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}