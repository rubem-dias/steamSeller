namespace SteamItemSeller.Services.SteamServices.Interfaces;

public interface IAuthentication
{
    public Task<string?> GetUserId(string sessionId, string steamLoginSecure);
}