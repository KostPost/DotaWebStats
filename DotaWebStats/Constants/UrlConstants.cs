namespace DotaWebStats.Constants;

public class UrlConstants
{
    public const string IndexPage = "https://localhost:7277/";
    
    
    
    private const string SteamCommunityBaseUrl = "https://steamcommunity.com";
    public static string SteamProfileUrl(string steamId) => $"{SteamCommunityBaseUrl}/profiles/{steamId}";

    
    private const string PlayerProfileBase = "/player";
    public static string PlayerProfileUrl(string id) => $"{PlayerProfileBase}/{id}";
}