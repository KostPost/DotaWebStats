using System.Net;

namespace DotaWebStats.Services;

using DotaWebStats.Constants;
using DotaWebStats.Models;

using System.Text.Json;
using Microsoft.Extensions.Configuration;

public interface IDotaDataService
{
    Task<UserDotaStats?> GetPlayerSummary(long id);

    Task<WinLoseStats?> GetPlayerWinLoss(long id);
}

public class DotaDataService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IDotaDataService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    

    public async Task<UserDotaStats?> GetPlayerSummary(long id)
    {
        if (IdDifference(id) == 0)
        {
            return null;
        }
        
        var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetPlayerStats(id));
        
        Console.WriteLine("API Link Player/{Id} \t" + ApiConstants.DotaApi.GetPlayerStats(id));

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
                    Dota2Id = id.ToString()
                    
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
    
    
    public async Task<WinLoseStats?> GetPlayerWinLoss(long id)
    {
        if (IdDifference(id) == 0)
        {
            return null;
        }
        
        var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetPlayerWinLoss(id));

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

    
    
    
    private string GetRankImagePath(int rankTier)
    {
        var tier = rankTier / 10;
        var stars = rankTier % 10;

        var rankNames = new Dictionary<int, string>
        {
            { 1, "herald" },
            { 2, "guardian" },
            { 3, "crusader" },
            { 4, "archon" },
            { 5, "legend" },
            { 6, "ancient" },
            { 7, "divine" },
            { 8, "immortal" }
        };

        if (tier != 8)
        {
            var rankName = rankNames.TryGetValue(tier, out var name) ? name : "unranked";
            return $"/DotaRanks/seasonal-rank-{rankName}-{stars}.png";
        }

        return $"/DotaRanks/seasonal-rank-immortal.png";
    }

    private string GetRankName(int rankTier)
    {
        var tier = rankTier / 10;
        var stars = rankTier % 10;

        string rankName = tier switch
        {
            1 => "Herald",
            2 => "Guardian",
            3 => "Crusader",
            4 => "Archon",
            5 => "Legend",
            6 => "Ancient",
            7 => "Divine",
            8 => "Immortal",
            _ => "Unranked"
        };

        if (tier != 8)
        {
            return $"{rankName} {stars}";
        }

        return $"Immortal Rank {_player.LeaderboardRank.Value}";
    }
    private long IdDifference(long id)
    {
        string userId = id.ToString();

        if (string.IsNullOrEmpty(userId))
        {
            return 0;
        }

        long dota2Id = 0;

       
        if (userId.Length == 9)
        {
           
            if (long.TryParse(userId, out dota2Id))
            {
                return dota2Id; 
            }
        }
        else if (userId.Length == 17)
        {
            if (long.TryParse(userId, out long steamId))
            {
                dota2Id = steamId - NumConstants.SteamIdToDota2IdDiff;
                return dota2Id;
            }
           
        }
       

        return 0; 
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