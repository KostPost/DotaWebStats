using DotaWebStats.Constants;
using DotaWebStats.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DotaWebStats.Services;

public interface ISteamAuthService
{
    Task InitializeAsync(HttpContext httpContext);
    string GetSteamLoginUrl();
    Task ProcessCallbackAsync(HttpContext httpContext);
    Task LogoutAsync(HttpContext httpContext);
    long? SteamId { get; }
    long? Dota2Id { get; }
    UserDotaStats? UserData { get; }
    Task SetUserDataAsync(UserDotaStats userData, HttpContext httpContext);
}

public class SteamAuthService : ISteamAuthService
{
    private const string RedirectUri = UrlConstants.IndexPage;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SteamAuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? SteamId { get; private set; }
    public long? Dota2Id { get; private set; }
    public UserDotaStats? UserData { get; private set; }

    private long? ConvertSteamIdToDota2Id(long steamId)
    {
        return steamId - NumConstats.SteamIdToDota2IdDiff;
    }

    public async Task InitializeAsync(HttpContext httpContext)
    {
        var steamIdCookie = httpContext.Request.Cookies["SteamId"];
        var dota2IdCookie = httpContext.Request.Cookies["Dota2Id"];
        var userDataCookie = httpContext.Request.Cookies["UserData"];

        if (long.TryParse(steamIdCookie, out long steamId))
        {
            SteamId = steamId;
        }

        if (long.TryParse(dota2IdCookie, out long dota2Id))
        {
            Dota2Id = dota2Id;
        }
        else if (SteamId.HasValue)
        {
            Dota2Id = ConvertSteamIdToDota2Id(SteamId.Value);
        }

        if (!string.IsNullOrEmpty(userDataCookie))
        {
            UserData = JsonSerializer.Deserialize<UserDotaStats>(userDataCookie);
        }
    }

    public string GetSteamLoginUrl()
    {
        var steamOpenIdUrl = "https://steamcommunity.com/openid/login";
        var parameters = new Dictionary<string, string>
        {
            { "openid.ns", "http://specs.openid.net/auth/2.0" },
            { "openid.mode", "checkid_setup" },
            {
                "openid.return_to",
                $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Authorization/Callback"
            },
            {
                "openid.realm",
                $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}"
            },
            { "openid.identity", "http://specs.openid.net/auth/2.0/identifier_select" },
            { "openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select" }
        };

        return
            $"{steamOpenIdUrl}?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";
    }

    public async Task ProcessCallbackAsync(HttpContext httpContext)
    {
        var steamIdParam = httpContext.Request.Query["openid.claimed_id"].FirstOrDefault();

        if (!string.IsNullOrEmpty(steamIdParam))
        {
            var steamIdString = steamIdParam.Split('/').Last();
            if (long.TryParse(steamIdString, out long steamId))
            {
                SteamId = steamId;
                Dota2Id = ConvertSteamIdToDota2Id(steamId);

                httpContext.Response.Cookies.Append("SteamId", steamId.ToString(),
                    new CookieOptions { HttpOnly = true, Secure = true });
                httpContext.Response.Cookies.Append("Dota2Id", Dota2Id.Value.ToString(),
                    new CookieOptions { HttpOnly = true, Secure = true });
            }
        }
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        SteamId = null;
        Dota2Id = null;
        UserData = null;
        httpContext.Response.Cookies.Delete("SteamId");
        httpContext.Response.Cookies.Delete("Dota2Id");
        httpContext.Response.Cookies.Delete("UserData");
    }

    public async Task SetUserDataAsync(UserDotaStats userData, HttpContext httpContext)
    {
        UserData = userData;
        var userDataJson = JsonSerializer.Serialize(userData);
        httpContext.Response.Cookies.Append("UserData", userDataJson,
            new CookieOptions { HttpOnly = true, Secure = true });
    }
}