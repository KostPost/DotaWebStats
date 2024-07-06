namespace DotaWebStats.Components.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SteamAuthService
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    private readonly IConfiguration _configuration;
    private const string RedirectUri = "https://localhost:1234/";

    public string SteamId { get; private set; }
    private string ApiKey;

    public SteamAuthService(NavigationManager navigationManager, IJSRuntime jsRuntime, IConfiguration configuration)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
        _configuration = configuration;
    }

    public async Task InitializeAsync()
    {
        ApiKey = _configuration["Steam:ApiKey"];
        var uri = new Uri(_navigationManager.Uri);
        var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var steamIdParam = queryParams["openid.claimed_id"];

        if (!string.IsNullOrEmpty(steamIdParam))
        {
            SteamId = steamIdParam.Split('/').Last();
        }
    }

    public void InitiateSteamAuth()
    {
        Console.WriteLine("qwe");
        var steamOpenIdUrl = "https://steamcommunity.com/openid/login";
        var parameters = new Dictionary<string, string>
        {
            {"openid.ns", "http://specs.openid.net/auth/2.0"},
            {"openid.mode", "checkid_setup"},
            {"openid.return_to", RedirectUri},
            {"openid.realm", "https://localhost:1234"},
            {"openid.identity", "http://specs.openid.net/auth/2.0/identifier_select"},
            {"openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select"}
        };

        var url = $"{steamOpenIdUrl}?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";
        _navigationManager.NavigateTo(url, forceLoad: true);
    }
}