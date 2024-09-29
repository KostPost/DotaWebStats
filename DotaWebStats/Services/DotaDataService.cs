using DotaWebStats.Constants;
using DotaWebStats.Models;

namespace Services;

using System.Text.Json;
using Microsoft.Extensions.Configuration;

public interface IDotaDataService
{
    Task<UserDotaStats?> GetPlayerSummary(long dotaId);

    Task<WinLoseStats?> GetPlayerWinLoss(long accountId);
}

public class DotaDataService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IDotaDataService
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
    
    public async Task<WinLoseStats?> GetPlayerWinLoss(long accountId)
    {
        var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetPlayerWinLoss(accountId));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var winLossData = JsonSerializer.Deserialize<WinLoseStats>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return winLossData;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize win/loss API response: {ex.Message}");
                return null;
            }
        }

        Console.WriteLine($"Win/Loss API request failed with status code: {response.StatusCode}");
        return null;
    }

    
    
    
}