namespace DotaWebStats.Constants
{
    public class ApiConstants
    {
        private const string DotaApiBaseUrl = "https://api.opendota.com/api";
        private const string SteamApiBaseUrl = "https://api.steampowered.com";
        private const string HeroImageBaseUrl = "https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/";

        public static string GetSteamApiKey(IConfiguration configuration)
        {
            var apiKey = configuration["Steam:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Steam API key is missing in the configuration.");
            }
            return apiKey;
        }

        public static class SteamApi
        {
            public static string GetPlayerSummaries(string apiKey, string steamId) => 
                $"{SteamApiBaseUrl}/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamId}";

            public static string GetOwnedGames(string apiKey, string steamId) =>
                $"{SteamApiBaseUrl}/IPlayerService/GetOwnedGames/v0001/?key={apiKey}&steamid={steamId}&format=json";
        }

        public static class DotaApi
        {
            public static string GetPlayerStats(long accountId) =>
                $"{DotaApiBaseUrl}/players/{accountId}";

            public static string GetRecentMatches(long accountId) =>
                $"{DotaApiBaseUrl}/players/{accountId}/recentMatches";

            public static string GetPlayerWinLoss(long accountId) =>
                $"{DotaApiBaseUrl}/players/{accountId}/wl";

            // New endpoint for retrieving Dota 2 heroes
            public static string GetHeroes() => $"{DotaApiBaseUrl}/heroes";
        }

        // New method to get hero image URL
        public static string GetHeroImageUrl(string heroName)
        {
            string formattedHeroName = heroName.ToLowerInvariant().Replace(" ", "_");
            return $"{HeroImageBaseUrl}{formattedHeroName}.png";
        }
    }
}
