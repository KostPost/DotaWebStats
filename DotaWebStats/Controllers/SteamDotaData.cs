using System.Text.Json;
using DotaWebStats.Constants;
using DotaWebStats.Models;

namespace DotaWebStats.Controllers;

public interface ISteamDotaData
{
    Task<UserDotaStats?> GetPlayerSummary(long dotaId);

    // Task<WinLoseStats?> GetPlayerWinLoss(long accountId);
}

public class SteamDotaData(IConfiguration configuration, IHttpClientFactory httpClientFactory) : ISteamDotaData
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<UserDotaStats?> GetPlayerSummary(long dota2Id)
    {
        var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetPlayerStats(dota2Id));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            try
            {
                var jsonDocument = JsonDocument.Parse(content);
                var root = jsonDocument.RootElement;

                var userDotaStats = new UserDotaStats
                {
                    Profile = JsonSerializer.Deserialize<Profile>(root.GetProperty("profile").GetRawText(), options),
                    RankTier = GetNullableInt(root, "rank_tier") ?? 0,
                    LeaderboardRank = GetNullableInt(root, "leaderboard_rank"),
                    Dota2Id = dota2Id.ToString()
                };

                return userDotaStats;
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
    
    private static int? GetNullableInt(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind == JsonValueKind.Number)
            {
                return property.GetInt32();
            }
        }
        return null;
    }

    
}