namespace DotaWebStats.Models;

public class RecentMatchesSummary
{
    public double WinRate { get; set; }

    public double AverageKills { get; set; }
    public double AverageDeaths { get; set; }
    public double AverageAssists { get; set; }
    public double AverageGoldPerMin { get; set; }
    public double AverageXpPerMin { get; set; }
    public double AverageLastHits { get; set; }
    public double AverageHeroDamage { get; set; }
    public double AverageHeroHealing { get; set; }
    public double AverageTowerDamage { get; set; }
    public double AverageDuration { get; set; }

    public int MaxKills { get; set; }
    public int MaxDeaths { get; set; }
    public int MaxAssists { get; set; }
    public int MaxGoldPerMin { get; set; }
    public int MaxXpPerMin { get; set; }
    public int MaxLastHits { get; set; }
    public int MaxHeroDamage { get; set; }
    public int MaxHeroHealing { get; set; }
    public int MaxTowerDamage { get; set; }
    public int MaxDuration { get; set; }
}
