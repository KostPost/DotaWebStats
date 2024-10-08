namespace DotaWebStats.Models;

public class PlayerViewModel
{
    public UserDotaStats UserStats { get; set; }
    public RecentMatchesSummary RecentMatchesSummary { get; set; } 
    
    public List<RecentMatches> RecentMatches { get; set; }
}
