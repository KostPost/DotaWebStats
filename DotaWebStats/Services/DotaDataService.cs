using System.Text.RegularExpressions;
using DotaWebStats.Model;
using DotaWebStats.Models.DotaData;
using DotaWebStats.Models.MatchesData;
using DotaWebStats.Services.Helpers;

namespace DotaWebStats.Services;

using Constants;
using Models;
using System.Text.Json;

public interface IDotaDataService
{
    Task<UserDotaStats?> GetPlayerSummary(long accountId);

    Task<WinLoseStats?> GetPlayerWinLoss(long accountId);

    Task<RecentMatchesSummary?> RecentMatchesSummary(long accountId);

    Task<List<RecentMatches>?> GetRecentMatches(long accountId);

    Task<MatchOverview?> GetMatchInfo(long matchId);
    
    Task<List<PlayerMatches>?> GetPlayerMatchAsync(long matchId);
}

public class DotaDataService(IHttpClientFactory httpClientFactory, ApiService apiService) : IDotaDataService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private readonly ApiService _apiService = apiService;


    public async Task<UserDotaStats?> GetPlayerSummary(long accountId)
    {
        if (DotaDataHelper.IdDifference(accountId) == 0)
        {
            return null;
        }

        var root = await _apiService.GetPlayerSummaryJsonAsync(accountId);

        if (root == null || !root.Value.TryGetProperty("profile", out var profileElement))
        {
            Console.WriteLine("Profile property not found in the response.");
            return null;
        }

        var profile = JsonSerializer.Deserialize<Profile>(profileElement.ToString());
        if (profile == null)
        {
            Console.WriteLine("Profile deserialization returned null.");
            return null;
        }

        return new UserDotaStats
        {
            Profile = profile,
            RankTier = DotaDataHelper.GetNullableInt(root.Value, "rank_tier") ?? 0,
            LeaderboardRank = DotaDataHelper.GetNullableInt(root.Value, "leaderboard_rank"),
            Dota2Id = accountId.ToString()
        };
    }

    public async Task<WinLoseStats?> GetPlayerWinLoss(long accountId)
    {
        if (DotaDataHelper.IdDifference(accountId) == 0)
        {
            return null;
        }

        var root = await _apiService.GetPlayerWinLossJsonAsync(accountId);
        if (root == null) return null;

        var winLossData = JsonSerializer.Deserialize<WinLoseStats>(root.Value.ToString());
        if (winLossData == null)
        {
            Console.WriteLine("Deserialization resulted in null WinLoseStats.");
            return null;
        }

        winLossData.WinRate = DotaDataHelper.CalculateWinRate(winLossData.Win, winLossData.Lose);
        return winLossData;
    }

    public async Task<RecentMatchesSummary?> RecentMatchesSummary(long accountId)
    {
        if (DotaDataHelper.IdDifference(accountId) == 0)
        {
            return null;
        }

        var root = await _apiService.GetRecentMatchesJsonAsync(accountId);
        if (root == null) return null;

        var recentMatches = JsonSerializer.Deserialize<List<RecentMatches>>(root.Value.ToString()) ??
                            new List<RecentMatches>();
        return DotaDataHelper.GetRecentAverageMaximum(recentMatches);
    }

    public async Task<List<RecentMatches>?> GetRecentMatches(long accountId)
    {
        if (DotaDataHelper.IdDifference(accountId) == 0)
        {
            return null;
        }

        var root = await _apiService.GetRecentMatchesJsonAsync(accountId);
        if (root == null) return null;

        var recentMatches = JsonSerializer.Deserialize<List<RecentMatches>>(root.Value.ToString());
        if (recentMatches == null)
        {
            Console.WriteLine("Deserialized recentMatches is null.");
            return null;
        }

        return recentMatches;
    }
    public async Task<MatchOverview?> GetMatchInfo(long matchId)
    {
        return await _apiService.GetMatchInfoAsync(matchId);
    }

    // public async Task<List<PlayerMatches>?> GetPlayerMatchAsync(long accountId)
    // {
    //     if (DotaDataHelper.IdDifference(accountId) == 0)
    //     {
    //         return null;
    //     }
    //
    //     var root = await _apiService.GetPlayerMatchesAsyncJsonAsync(accountId);
    //
    //     if (root == null)
    //     {
    //         return null;
    //     }
    //
    //     // Ensure root is a valid JSON string
    //     var rootString = root.ToString();
    //     if (string.IsNullOrEmpty(rootString))
    //     {
    //         return null;
    //     }
    //
    //     Console.WriteLine("Raw API response: " + rootString); // Log the raw response for inspection
    //
    //     try
    //     {
    //         var playerMatches = JsonSerializer.Deserialize<List<PlayerMatches>>(rootString, new JsonSerializerOptions
    //         {
    //             PropertyNameCaseInsensitive = true  // Make sure case differences in property names are handled
    //         });
    //
    //         if (playerMatches == null)
    //         {
    //             Console.WriteLine("Deserialized playerMatches is null.");
    //             return null;
    //         }
    //
    //         return playerMatches;
    //     }
    //     catch (JsonException ex)
    //     {
    //         Console.WriteLine($"Error during deserialization: {ex.Message}");
    //         return null;
    //     }
    // }

    public async Task<List<PlayerMatches>?> GetPlayerMatchAsync(long accountId)
    {
        if (DotaDataHelper.IdDifference(accountId) == 0)
        {
            return null;
        }

        var jsonString = await _apiService.GetPlayerMatchesAsyncJsonAsync(accountId);

        if (string.IsNullOrEmpty(jsonString))
        {
            return null;
        }


        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new NullableIntConverter() }
            };

            var playerMatches = JsonSerializer.Deserialize<List<PlayerMatches>>(jsonString, options);
        
            if (playerMatches == null)
            {
                Console.WriteLine("Deserialized playerMatches is null.");
                return null;
            }

            return playerMatches;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error during deserialization: {ex.Message}");
            return null;
        }
    }

    
}