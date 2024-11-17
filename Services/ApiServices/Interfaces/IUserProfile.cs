using SteamItemSeller.Services.Dtos;

namespace SteamItemSeller.Services.SteamServices.Interfaces;

public interface IUserProfile
{
    public Task<UserData> GetUserProfileData(string sessionId, string steamLoginSecure, string userProfile);
}