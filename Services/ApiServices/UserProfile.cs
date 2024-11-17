using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SteamItemSeller.Services.Dtos;
using SteamItemSeller.Services.SteamServices.Interfaces;

namespace SteamItemSeller.Services.ApiServices;
public class UserProfile(HttpClient httpClient) : IUserProfile
{
    private readonly CookieContainer _cookieContainer = new CookieContainer();
    private HttpResponseMessage _response = new HttpResponseMessage();

    private const string _baseUri = "https://steamcommunity.com/id";

    public async Task<UserData> GetUserProfileData(string sessionId, string steamLoginSecure, string userProfile)
    {
        try
        {
            var userData = new UserData();
            var url = $"{_baseUri}/{userProfile}";

            AddInitialCookies(url, "sessionid", sessionId);
            AddInitialCookies(url, "steamLoginSecure", steamLoginSecure);

            _response = await httpClient.GetAsync(url);

            var currentCookies = await GetCookiesAsString();

            userData.ProfileUrl = url;
            userData.Cookie = currentCookies;

            return userData;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private Task<string> GetCookiesAsString()
    {
        var cookieCollection = _cookieContainer.GetCookies(new Uri(_baseUri));
        var cookieHeader = new StringBuilder();

        foreach (Cookie cookie in cookieCollection)
        {
            if (cookieHeader.Length > 0)
                cookieHeader.Append("; ");
            cookieHeader.Append($"{cookie.Name}={cookie.Value}");
        }

        return Task.FromResult(cookieHeader.ToString());
    }
    private void AddInitialCookies(string url, string name, string value)
    {
        _cookieContainer.Add(new Uri(url), new Cookie(name, value));
    }
}