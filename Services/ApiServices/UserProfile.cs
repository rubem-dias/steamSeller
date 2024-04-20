﻿using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SteamItemSeller.Services.SteamServices.Interfaces;
namespace SteamItemSeller.Services.SteamServices;
public class Authentication : IAuthentication
{
    private readonly HttpClient _httpClient;
    private readonly CookieContainer _cookieContainer = new CookieContainer();
    private string _baseUri = "https://steamcommunity.com/";
    private HttpResponseMessage _response = new HttpResponseMessage();
    private string _document = String.Empty;
    
    public Authentication(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<UserProfileResponse> GetUserProfile(string sessionId, string steamLoginSecure)
    {
        AddInitialCookies(_baseUri, "sessionId", sessionId);
        AddInitialCookies(_baseUri, "steamLoginSecure", steamLoginSecure);

        _response = await _httpClient.GetAsync(_baseUri);

        UpdateCookieContainer(_response, new Uri(_baseUri));
        
        var getCurrentProfile = await GetCurrentProfile();
        return getCurrentProfile;
    }
    private async Task<UserProfileResponse> GetCurrentProfile()
    {
        UserProfileResponse responseProfile = new();
        
        ConfigureHttpClient();
        
        _document = await _httpClient
            .GetAsync(_baseUri).Result.Content
            .ReadAsStringAsync();

        var steamId = Regex.Match(_document, @"g_steamID\s*=\s*""(\d+)"";")
            .Groups[1]
            .Value;
        
        var profileUrl = Regex.Match(_document, "href\\s*=\\s*\"(https://steamcommunity\\.com/id/[^\"]*)\"")
            .Groups[1]
            .Value;

        responseProfile.ProfileUrl = profileUrl;
        responseProfile.SteamId = steamId;
        responseProfile.CurrentSteamToken = GetCookiesAsString();

        return responseProfile;
    }
    private string GetCookiesAsString()
    {

        var cookieCollection = _cookieContainer.GetCookies(new Uri(_baseUri));
        var cookieHeader = new StringBuilder();

        foreach (Cookie cookie in cookieCollection)
        {
            if (cookieHeader.Length > 0)
                cookieHeader.Append("; ");
            cookieHeader.Append($"{cookie.Name}={cookie.Value}");
        }

        return cookieHeader.ToString();

    }
    private void ConfigureHttpClient()
    {
        string cookiesAsString = GetCookiesAsString();

        if (!string.IsNullOrEmpty(cookiesAsString))
        {
            _httpClient.DefaultRequestHeaders.Remove("Cookie");
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookiesAsString);
        }

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
    }
    private void UpdateCookieContainer(HttpResponseMessage _response, Uri requestUri)
    {
        var responseCookies = _cookieContainer.GetCookies(requestUri);

        var newCookies = _response.Headers
            .Where(h => h.Key.Equals("Set-Cookie"))
            .SelectMany(h => h.Value)
            .ToList();

        foreach (var cookieString in newCookies)
        {
            var cookieParts = cookieString.Split(';').First();
            var cookieName = cookieParts.Split('=')[0];
            var cookieValue = cookieParts.Split('=')[1];

            var cookie = new Cookie(cookieName, cookieValue);

            var existingCookie = _cookieContainer.GetCookies(requestUri).Cast<Cookie>()
                                .FirstOrDefault(c => c.Name == cookieName);

            if (existingCookie != null)
            {
                existingCookie.Value = cookieValue;
            }
            else
            {
                var newCookie = new Cookie(cookieName, cookieValue);
                _cookieContainer.Add(requestUri, newCookie);
            }
        }
    }
    private void AddInitialCookies(string url, string name, string value)
    {
        _cookieContainer.Add(new Uri(url), new Cookie(name, value));
    }
}