using DotaWebStats.Models.MatchesData;

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

    Task<Match?> GetMatchInfo(long matchId);
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

        recentMatches = DotaDataHelper.SetRankName(recentMatches);
        recentMatches = DotaDataHelper.SetGameModeName(recentMatches);
        return DotaDataHelper.GetCorrectStats(recentMatches);
    }

    public async Task<Match?> GetMatchInfo(long matchId)
    {
        return await _apiService.GetMatchInfoAsync(matchId);
    }
}