namespace Services.Constants;

public static class UrlConstants
{
    public const string IndexPage = "https://localhost:1234/";
    
    
    
    private const string SteamCommunityBaseUrl = "https://steamcommunity.com";
    public static string SteamProfileUrl(string steamId) => $"{SteamCommunityBaseUrl}/profiles/{steamId}";

    
    private const string PlayerProfileBase = "/player";
    public static string PlayerProfileUrl(string id) => $"{PlayerProfileBase}/{id}";
    
    
    
}