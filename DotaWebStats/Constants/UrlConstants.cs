namespace DotaWebStats.Constants;

public static class UrlConstants
{
    public const string IndexPage = "https://localhost:7277/";


    public static string PlayerOverviewPage(string id) => $"http://localhost:5103/player/{id}/overview";
    
    public static string MatchOverviewPage(string id) => $"http://localhost:5103/match/{id}";


    private const string SteamCommunityBaseUrl = "https://steamcommunity.com";
    public static string SteamProfileUrl(string steamId) => $"{SteamCommunityBaseUrl}/profiles/{steamId}";


    private const string PlayerProfileBase = "/player";
    public static string PlayerProfileUrl(string id) => $"{PlayerProfileBase}/{id}";
}