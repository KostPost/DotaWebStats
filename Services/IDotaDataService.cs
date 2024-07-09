using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelClasses;

public interface IDotaDataService
{
    Task<SteamUserData.Player> GetPlayerSummary(string steamId);
    Task<MatchStats> GetMatchStats(string steamId);
    Task<string> GetDotaRank(string steamId);
}

public class DotaDataService : IDotaDataService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<DotaDataService> _logger;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.steampowered.com/";

    public DotaDataService(IConfiguration configuration, HttpClient httpClient, ILogger<DotaDataService> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = _configuration["Steam:ApiKey"];

        if (string.IsNullOrEmpty(_apiKey))
        {
            _logger.LogError("Steam API key is not configured.");
            throw new InvalidOperationException("Steam API key is not configured.");
        }
    }

    public async Task<SteamUserData.Player> GetPlayerSummary(string steamId)
    {
        try
        {
            var url = $"{BaseUrl}ISteamUser/GetPlayerSummaries/v2/?key={_apiKey}&steamids={steamId}";
            _logger.LogInformation($"Requesting player summary: {url}");

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Received response: {content}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var playerSummary = JsonSerializer.Deserialize<SteamUserData.SteamPlayerSummary>(content, options);

            if (playerSummary?.Response?.Players == null || !playerSummary.Response.Players.Any())
            {
                _logger.LogWarning("No player data found in the response.");
                return null;
            }

            return playerSummary.Response.Players.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching player summary for SteamID: {steamId}");
            throw;
        }
    }

    public async Task<MatchStats> GetMatchStats(string steamId)
    {
        try
        {
            var url =
                $"{BaseUrl}IDOTA2Match_570/GetMatchHistory/v1/?key={_apiKey}&account_id={steamId}&matches_requested=100";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var matchHistory = JsonSerializer.Deserialize<MatchHistoryResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (matchHistory?.Result?.Matches == null)
            {
                throw new Exception("Failed to retrieve match history");
            }

            int totalMatches = matchHistory.Result.Matches.Count;
            int wins = matchHistory.Result.Matches.Count(m => m.RadiantWin == (m.PlayerSlot < 128));

            return new MatchStats
            {
                TotalMatches = totalMatches,
                Wins = wins,
                Losses = totalMatches - wins
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching match stats for SteamID: {steamId}");
            throw;
        }
    }

    public async Task<string> GetDotaRank(string steamId)
    {
        try
        {
            var url = $"{BaseUrl}IDOTA2Match_570/GetPlayerStats/v1/?key={_apiKey}&account_id={steamId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var playerStats = JsonSerializer.Deserialize<PlayerStatsResponse>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (playerStats?.Result?.MmrEstimate?.Estimate == null)
            {
                return "Unknown";
            }

            int mmr = playerStats.Result.MmrEstimate.Estimate;
            return GetRankFromMMR(mmr);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching Dota rank for SteamID: {steamId}");
            throw;
        }
    }

    private string GetRankFromMMR(int mmr)
    {
        if (mmr < 720) return "Herald";
        if (mmr < 1560) return "Guardian";
        if (mmr < 2400) return "Crusader";
        if (mmr < 3240) return "Archon";
        if (mmr < 4080) return "Legend";
        if (mmr < 5000) return "Ancient";
        if (mmr < 5600) return "Divine";
        return "Immortal";
    }

// Add these classes to deserialize the API responses
    public class MatchHistoryResponse
    {
        public MatchHistoryResult Result { get; set; }
    }

    public class MatchHistoryResult
    {
        public List<Match> Matches { get; set; }
    }

    public class Match
    {
        public long MatchId { get; set; }
        public bool RadiantWin { get; set; }
        public int PlayerSlot { get; set; }
    }

    public class PlayerStatsResponse
    {
        public PlayerStatsResult Result { get; set; }
    }

    public class PlayerStatsResult
    {
        public MmrEstimate MmrEstimate { get; set; }
    }

    public class MmrEstimate
    {
        public int Estimate { get; set; }
    }
}