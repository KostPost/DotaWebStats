using Microsoft.Extensions.Configuration;
using System.Text.Json;
using ModelClasses;
using Services.Constants;

namespace Services
{
    public interface ISteamDataService
    {
        Task<SteamUserData.Player?> GetPlayerSummary(long steamId);
    }

    public class SteamDataService : ISteamDataService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public SteamDataService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        
        public async Task<SteamUserData.Player?> GetPlayerSummary(long steamId)
        {
            var steamApiKey = ApiConstants.GetSteamApiKey(_configuration);

            var response = await _httpClient.GetAsync(ApiConstants.SteamApi.GetPlayerSummaries(steamApiKey, steamId.ToString()));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                try
                {
                    var playerSummary = JsonSerializer.Deserialize<SteamUserData.SteamPlayerSummary>(content, options);
                    var player = playerSummary?.Response.Players.FirstOrDefault();

                    if (player != null)
                    {
                        var tempDota2Id = steamId - NumConstats.SteamIdToDota2IdDiff;

                        player.Dota2Id = tempDota2Id.ToString();
                        return player;
                    }

                    return null;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Failed to deserialize API response: {ex.Message}");
                    return null;
                }
            }
    
            Console.WriteLine($"API request failed with status code: {response.StatusCode}");
            return null;
        }
    }
}