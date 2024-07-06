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
        Task<SteamUserData.Player> GetPlayerSummary(string steamId);
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

        public async Task<SteamUserData.Player> GetPlayerSummary(string steamId)
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
                    return player;
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
