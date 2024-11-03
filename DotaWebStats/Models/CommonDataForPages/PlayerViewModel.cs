using System.Collections.Generic;

namespace DotaWebStats.Models
{
    public class PlayerViewModel
    {
        public UserDotaStats UserStats { get; set; } = new UserDotaStats
        {
            Dota2Id = string.Empty
        };        
        
        public RecentMatchesSummary RecentMatchesSummary { get; set; } = new RecentMatchesSummary();
        public List<RecentMatches> RecentMatches { get; set; } = new List<RecentMatches>();
    }
}