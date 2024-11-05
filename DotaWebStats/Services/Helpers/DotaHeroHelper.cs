using System.Text.Json;
using DotaWebStats.Constants;
using DotaWebStats.Models;

namespace DotaWebStats.Services.Helpers;

public class DotaHeroHelper
{
    private static readonly Dictionary<int, (string Name, string LocalizedName)> HeroDictionary = new();
    private static readonly Dictionary<int, string> HeroNamesDictionary = new();
    private static readonly Lazy<Task> InitializeTask = new Lazy<Task>(InitializeAsyncInternal);

    public static Task InitializeAsync()
    {
        return InitializeTask.Value;
    }

    private static async Task InitializeAsyncInternal()
    {
        if (HeroDictionary.Any()) return;

        var heroes = await FetchHeroesFromApiAsync();
        if (heroes == null || !heroes.Any()) return;

        foreach (var hero in heroes)
        {
            var heroName = hero.Name.Replace("npc_dota_hero_", "").ToLower();
            HeroDictionary[hero.Id] = (heroName, hero.LocalizedName);
            HeroNamesDictionary[hero.Id] = heroName;
        }
    }

    private static async Task<List<DotaHero>?> FetchHeroesFromApiAsync()
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetStringAsync(ApiConstants.DotaApi.GetHeroes());
            return JsonSerializer.Deserialize<List<DotaHero>>(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching heroes: {ex.Message}");
            return null;
        }
    }

    public static string GetHeroName(int heroId)
    {
        return HeroNamesDictionary.TryGetValue(heroId, out var heroName) ? heroName : "Unknown Hero";
    }
    

    

    public static string GetHeroImage(string heroName)
    {
        return ApiConstants.GetHeroImageUrl(heroName);
    }
    
    public static string GetHeroImage(int heroId)
    {
        return ApiConstants.GetHeroImageUrl(GetHeroName(heroId));
    }

    public static void SetHeroInfo(RecentMatches match)
    {
        if (HeroDictionary.TryGetValue(match.HeroId, out var heroInfo))
        {
            match.HeroName = heroInfo.Name;
            match.LocalizedName = heroInfo.LocalizedName;
            match.HeroImageUrl = ApiConstants.GetHeroImageUrl(heroInfo.Name);
        }
    }
}