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

    private const string BaseUri = "https://steamcommunity.com/";
    private string _document = string.Empty;

    public async Task<UserData> GetUserProfileData(string sessionId, string steamLoginSecure)
    {
        try
        {
            var userData = new UserData();

            AddInitialCookies(BaseUri, "sessionid", sessionId);
            AddInitialCookies(BaseUri, "steamLoginSecure", steamLoginSecure);

            _response = await httpClient.GetAsync(BaseUri);

            var profileUri = await GetProfileUri();
            var currentCookies = await GetCookiesAsString();

            if (string.IsNullOrEmpty(profileUri) || string.IsNullOrEmpty(currentCookies))
            {
                throw new Exception("Verify your credentials and try again");
            }

            userData.ProfileUrl = profileUri;
            userData.Cookie = currentCookies;

            return userData;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private async Task<string> GetProfileUri()
    {
        ConfigureHttpClient();
        
        _document = await httpClient
            .GetAsync(BaseUri).Result.Content
            .ReadAsStringAsync();
        
        var profileUrl = Regex.Match(_document, "href\\s*=\\s*\"(https://steamcommunity\\.com/id/[^\"]*)\"")
            .Groups[1]
            .Value;

        return profileUrl;
    }
    private Task<string> GetCookiesAsString()
    {
        var cookieCollection = _cookieContainer.GetCookies(new Uri(BaseUri));
        var cookieHeader = new StringBuilder();

        foreach (Cookie cookie in cookieCollection)
        {
            if (cookieHeader.Length > 0)
                cookieHeader.Append("; ");
            cookieHeader.Append($"{cookie.Name}={cookie.Value}");
        }

        return Task.FromResult(cookieHeader.ToString());
    }
    private async void ConfigureHttpClient()
    {
        var cookiesAsString = await GetCookiesAsString();

        if (!string.IsNullOrEmpty(cookiesAsString))
        {
            httpClient.DefaultRequestHeaders.Remove("Cookie");
            httpClient.DefaultRequestHeaders.Add("Cookie", cookiesAsString);
        }

        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
    }
    private void AddInitialCookies(string url, string name, string value)
    {
        _cookieContainer.Add(new Uri(url), new Cookie(name, value));
    }
}