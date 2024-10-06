namespace DotaWebStats.Services;

using Constants;
using Models;

using System.Text.Json;

public interface IDotaDataService
{
    Task<UserDotaStats?> GetPlayerSummary(long id);

    Task<WinLoseStats?> GetPlayerWinLoss(long id);

    Task<RecentMatchesSummary> GetRecentMatches(long accountId);

    string GetRankName(int rankTier);
    
    string GetRankImagePath(int rankTier);
}

public class DotaDataService(IHttpClientFactory httpClientFactory) : IDotaDataService
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

        long dotaId = id;
        
        var response = await _httpClient.GetAsync(ApiConstants.DotaApi.GetPlayerWinLoss(dotaId));
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API request failed with status code: {response.StatusCode}");
            return null;
        }
        
        
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("API response content is empty.");
            return null;
        }

        try
        {
            var winLossData = JsonSerializer.Deserialize<WinLoseStats>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (winLossData == null)
            {
                Console.WriteLine("Deserialization resulted in null WinLoseStats.");
                return null;
            }

            winLossData.WinRate = CalculateWinRate(winLossData.Win, winLossData.Lose);

            return winLossData;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize win/loss API response: {ex.Message}");
            return null;
        }
    }
    public async Task<RecentMatchesSummary> GetRecentMatches(long accountId)
    {
        if (IdDifference(accountId) == 0)
        {
            return null;
        }

        var response = await _httpClient.GetAsync($"https://api.opendota.com/api/players/{accountId}/recentMatches");
    
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API request failed with status code: {response.StatusCode}");
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("API response content is empty.");
            return null;
        }

        try
        {
            var recentMatches = JsonSerializer.Deserialize<List<RecentMatches>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });



            return GetRecentAverageMaximum(recentMatches);





        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize recent matches API response: {ex.Message}");
            return null;
        }
    }

  
    
    
    
    
    public string GetRankImagePath(int rankTier)
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
        
        return "/DotaRanks/seasonal-rank-immortal.png";
    }
    public string GetRankName(int rankTier)
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
    
        // return $"Immortal Rank {_player.LeaderboardRank.Value}";
        
        return $"Immortal Rank";

    }

    private RecentMatchesSummary GetRecentAverageMaximum(List<RecentMatches> recentMatches)
    {
        if (recentMatches == null || recentMatches.Count == 0)
        {
            return new RecentMatchesSummary();
        }

        var recentMatchesSummary = new RecentMatchesSummary
        {
            MaxKills = recentMatches.Max(m => m.Kills),
            MaxDeaths = recentMatches.Max(m => m.Deaths),
            MaxAssists = recentMatches.Max(m => m.Assists),
            MaxGoldPerMin = recentMatches.Max(m => m.GoldPerMin),
            MaxXpPerMin = recentMatches.Max(m => m.XpPerMin),
            MaxLastHits = recentMatches.Max(m => m.LastHits),
            MaxHeroDamage = recentMatches.Max(m => m.HeroDamage),
            MaxHeroHealing = recentMatches.Max(m => m.HeroHealing),
            MaxTowerDamage = recentMatches.Max(m => m.TowerDamage),
            MaxDuration = recentMatches.Max(m => m.Duration),
            
            
            
            AverageKills = recentMatches.Average(m => m.Kills),
            AverageDeaths = recentMatches.Average(m => m.Deaths),
            AverageAssists = recentMatches.Average(m => m.Assists),
            AverageGoldPerMin = recentMatches.Average(m => m.GoldPerMin),
            AverageXpPerMin = recentMatches.Average(m => m.XpPerMin),
            AverageLastHits = recentMatches.Average(m => m.LastHits),
            AverageHeroDamage = recentMatches.Average(m => m.HeroDamage),
            AverageHeroHealing = recentMatches.Average(m => m.HeroHealing),
            AverageTowerDamage = recentMatches.Average(m => m.TowerDamage),
            AverageDuration = recentMatches.Average(m => m.Duration)
        };

        int wins = 0, losses = 0;
        foreach (var match in recentMatches)
        {
            if (match.PlayerSlot < 128 && match.RadiantWin == true)
            {
                losses++;
            }
            else if((match.PlayerSlot >= 128 && match.RadiantWin == false))
            {
                wins++;
            }
            else
            {
                wins++;
            }
            
            
        }
        
        Console.WriteLine($"\n\nLose {losses} + \t Wins {wins}\n\n");

        recentMatchesSummary.WinRate = CalculateWinRate(wins, losses);

        return recentMatchesSummary;
    }
    
    
    private double CalculateWinRate(int wins, int losses)
    {
        if (wins + losses == 0)
        {
            return 0; 
        }

        return Math.Round((double)wins / (wins + losses) * 100, 2);
    }
    private long IdDifference(long id)
    {
        string userId = id.ToString();

        if (string.IsNullOrEmpty(userId))
        {
            return 0;
        }

        long dota2Id;

       
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

