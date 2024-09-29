namespace DotaWebStats.Models;

public class UserDotaStats
{
    public Profile Profile { get; set; }
    public int RankTier { get; set; }
    public int? LeaderboardRank { get; set; }  
    public required string? Dota2Id { get; set; }
}