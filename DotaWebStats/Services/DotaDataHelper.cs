using System.Text.Json;
using DotaWebStats.Constants;
using DotaWebStats.Models;

namespace DotaWebStats.Services;

public class DotaDataHelper 
{
    public static List<RecentMatches> SetGameModeName(List<RecentMatches> recentMatchesList)
    {
        foreach (var match in recentMatchesList)
        {
            match.GameModeName = match.GameMode switch
            {
                0 => "Unknown",
                1 => "All Pick",
                2 => "Captains Mode",
                3 => "Random Draft",
                4 => "Single Draft",
                5 => "All Random",
                6 => "Intro",
                7 => "Diretide",
                8 => "Reverse Captains Mode",
                9 => "Greeviling",
                10 => "Tutorial",
                11 => "Mid Only",
                12 => "Least Played",
                13 => "Limited Heroes",
                14 => "Compendium Matchmaking",
                15 => "Custom",
                16 => "Captains Draft",
                17 => "Balanced Draft",
                18 => "Ability Draft",
                19 => "Event",
                20 => "All Random Deathmatch",
                21 => "1v1 Mid",
                22 => "All Draft",
                23 => "Turbo",
                24 => "Mutation",
                _ => "Unknown"
            };
        }
    
        return recentMatchesList;
    }
    
    // public static RecentMatches SetGameModeName(RecentMatches recentMatchesList)
    // {
    //     foreach (var match in recentMatchesList)
    //     {
    //         match.GameModeName = match.GameMode switch
    //         {
    //             0 => "Unknown",
    //             1 => "All Pick",
    //             2 => "Captains Mode",
    //             3 => "Random Draft",
    //             4 => "Single Draft",
    //             5 => "All Random",
    //             6 => "Intro",
    //             7 => "Diretide",
    //             8 => "Reverse Captains Mode",
    //             9 => "Greeviling",
    //             10 => "Tutorial",
    //             11 => "Mid Only",
    //             12 => "Least Played",
    //             13 => "Limited Heroes",
    //             14 => "Compendium Matchmaking",
    //             15 => "Custom",
    //             16 => "Captains Draft",
    //             17 => "Balanced Draft",
    //             18 => "Ability Draft",
    //             19 => "Event",
    //             20 => "All Random Deathmatch",
    //             21 => "1v1 Mid",
    //             22 => "All Draft",
    //             23 => "Turbo",
    //             24 => "Mutation",
    //             _ => "Unknown"
    //         };
    //     }
    //
    //     return recentMatchesList;
    // }



    
    


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

    public static string GetRankName(int rankTier)
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


    public static List<RecentMatches> GetCorrectStats(List<RecentMatches> recentMatchesList)
    {
        if (recentMatchesList.Count == 0)
        {
            Console.WriteLine("recentMatchesList is null, returning an empty list.");
            return new List<RecentMatches>(); 
        }

        using var httpClient = new HttpClient();

        var response = httpClient.GetStringAsync(ApiConstants.DotaApi.GetHeroes()).Result;
        var heroes = JsonSerializer.Deserialize<List<DotaHero>>(response);
        
        if (heroes == null || !heroes.Any())
        {
            Console.WriteLine("Heroes data is null or empty.");
            return recentMatchesList; 
        }

        var heroDictionary = new Dictionary<int, (string Name, string LocalizedName)>();
        foreach (var hero in heroes)
        {
            var heroName = hero.Name.Replace("npc_dota_hero_", "");

            heroDictionary[hero.Id] = (heroName.ToLower(), hero.LocalizedName);
        }

        foreach (var match in recentMatchesList)
        {
            if (heroDictionary.TryGetValue(match.HeroId, out var heroInfo))
            {
                match.HeroName = heroInfo.Name;
                match.LocalizedName = heroInfo.LocalizedName;
                match.HeroImageUrl = ApiConstants.GetHeroImageUrl(heroInfo.Name); 
            }
        }

        recentMatchesList = SetWinLoss(recentMatchesList);

        return recentMatchesList;
    }



    public static RecentMatchesSummary GetRecentAverageMaximum(List<RecentMatches> recentMatches)
    {
        if (recentMatches.Count == 0)
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
            if (match.PlayerSlot < 128)
            {
                if (match.RadiantWin)
                {
                    wins++;
                }
                else
                {
                    losses++;
                }
            }
            else
            {
                if (match.RadiantWin)
                {
                    losses++;
                }
                else
                {
                    wins++;
                }
            }
        }

        Console.WriteLine($"\n\nLose {losses} + \t Wins {wins}\n\n");

        recentMatchesSummary.WinRate = CalculateWinRate(wins, losses);

        return recentMatchesSummary;
    }

    private static List<RecentMatches> SetWinLoss(List<RecentMatches> recentMatches)
    {
        foreach (var match in recentMatches)
        {
            if (match.PlayerSlot < 128)
            {
                if (match.RadiantWin)
                {
                    match.IsPlayerWin = true;
                }
                else
                {
                    match.IsPlayerWin = false;
                }
            }
            else
            {
                if (match.RadiantWin)
                {
                    match.IsPlayerWin = false;
                }
                else
                {
                    match.IsPlayerWin = true;
                }
            }
        }

        return recentMatches;
    }
    public static RecentMatches SetWinLoss(RecentMatches recentMatch)
    {
        if (recentMatch.PlayerSlot < 128)
        {
            if (recentMatch.RadiantWin)
            {
                recentMatch.IsPlayerWin = true;
            }
            else
            {
                recentMatch.IsPlayerWin = false;
            }
        }
        else
        {
            if (recentMatch.RadiantWin)
            {
                recentMatch.IsPlayerWin = false;
            }
            else
            {
                recentMatch.IsPlayerWin = true;
            }
        }

        return recentMatch;
    }
    
    
    public static double CalculateWinRate(int wins, int losses)
    {
        if (wins + losses == 0)
        {
            return 0;
        }

        return Math.Round((double)wins / (wins + losses) * 100, 2);
    }

    public static long IdDifference(long id)
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

    public static int? GetNullableInt(JsonElement element, string propertyName)
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

    public static List<RecentMatches> SetRankName(List<RecentMatches> recentMatchesList)
    {
        foreach (var match in recentMatchesList)
        {
            match.AverageRankName = GetRankName(match.AverageRank ?? 0);
        }

        return recentMatchesList;
    }
}