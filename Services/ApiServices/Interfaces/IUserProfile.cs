namespace SteamItemSeller.Services.SteamServices.Interfaces;

public interface IAuthentication
{
    public Task<UserProfileResponse> GetUserProfile(string sessionId, string steamLoginSecure);
}