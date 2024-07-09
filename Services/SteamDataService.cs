using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ModelClasses;

namespace Services
{
    public interface ISteamDataService
    {
        Task<SteamUserData.Player> GetPlayerSummary(long steamId);
    }

    public class SteamDataService : ISteamDataService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private const long SteamIdToDota2IdDiff = 76561197960265728;

        public SteamDataService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }
        
        public async Task<SteamUserData.Player> GetPlayerSummary(long steamId)
        {
            string steamApiKey = _configuration["Steam:ApiKey"];

            var response = await _httpClient.GetAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={steamApiKey}&steamids={steamId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                try
                {
                    var playerSummary = JsonSerializer.Deserialize<SteamUserData.SteamPlayerSummary>(content, options);

                    var player = playerSummary?.Response?.Players?.FirstOrDefault();
                    if (player != null)
                    {
                        // Convert the steamid from string to long
                        if (long.TryParse(player.Steamid, out long parsedSteamId))
                        {
                            player.Dota2Id = parsedSteamId - SteamIdToDota2IdDiff;
                        }
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
            else
            {
                Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                return null;
            }
        }
    }
}
