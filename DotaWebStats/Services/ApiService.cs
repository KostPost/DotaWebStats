using System.Text.Json;
using DotaWebStats.Constants;
using DotaWebStats.Models.MatchesData;
using DotaWebStats.Services.Helpers;

namespace DotaWebStats.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<JsonElement?> GetPlayerSummaryJsonAsync(long accountId)
        {
            return await GetJsonAsync(ApiConstants.DotaApi.GetPlayerStats(accountId), "PlayerSummary");
        }

        public async Task<JsonElement?> GetPlayerWinLossJsonAsync(long accountId)
        {
            return await GetJsonAsync(ApiConstants.DotaApi.GetPlayerWinLoss(accountId), "Win/Loss");
        }

        public async Task<JsonElement?> GetRecentMatchesJsonAsync(long accountId)
        {
            return await GetJsonAsync(ApiConstants.DotaApi.GetRecentMatches(accountId), "RecentMatches");
        }

        // public async Task<List<PlayerMatches>?> GetPlayerMatchesAsyncJsonAsync(long accountId)
        // {
        //     var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetMatches(accountId));
        //
        //     if (!response.IsSuccessStatusCode)
        //     {
        //         Console.WriteLine($"API request for matches failed with status code: {response.StatusCode}");
        //         return null;
        //     }
        //
        //     var content = await response.Content.ReadAsStringAsync();
        //
        //     try
        //     {
        //         var jsonOptions = new JsonSerializerOptions
        //         {
        //             Converters = { new NullableIntConverter() } // Add the converter here
        //         };
        //
        //         var playerMatches = JsonSerializer.Deserialize<List<PlayerMatches>>(content, jsonOptions);
        //
        //         return playerMatches;
        //     }
        //     catch (JsonException ex)
        //     {
        //         Console.WriteLine($"Failed to deserialize matches API response: {ex.Message}");
        //         return null;
        //     }
        // }
        
        public async Task<string?> GetPlayerMatchesAsyncJsonAsync(long accountId)
        {
            var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetMatches(accountId));

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API request for matches failed with status code: {response.StatusCode}");
                return null;
            }

            // Return the raw JSON string directly
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<MatchOverview?> GetMatchInfoAsync(long matchId)
        {
            var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetMatch(matchId));

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MatchOverview>(content);
        }

        private async Task<JsonElement?> GetJsonAsync(string url, string description)
        {
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API request for {description} failed with status code: {response.StatusCode}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine($"{description} API response content is empty.");
                return null;
            }

            try
            {
                var jsonDocument = JsonDocument.Parse(content);
                return jsonDocument.RootElement.Clone();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize {description} API response: {ex.Message}");
                return null;
            }
        }
    }
}
