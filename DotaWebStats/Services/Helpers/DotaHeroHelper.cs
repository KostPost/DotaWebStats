using System.Text.Json;
using DotaWebStats.Constants;
using DotaWebStats.Models;

namespace DotaWebStats.Services.Helpers;

public class DotaHeroHelper
{
    private static readonly Dictionary<int, string> HeroLocalizedNameDictionary = new();
    private static readonly Dictionary<int, string> HeroInternalNameDictionary = new();
    private static readonly Lazy<Task> InitializeTask = new Lazy<Task>(InitializeAsyncInternal);

    public static Task InitializeAsync()
    {
        return InitializeTask.Value;
    }

    private static async Task InitializeAsyncInternal()
    {
        if (HeroLocalizedNameDictionary.Any()) return;

        var heroesDictionary = await FetchHeroesFromApiAsync();
        if (heroesDictionary == null || !heroesDictionary.Any()) return;

        foreach (var hero in heroesDictionary)
        {
            HeroLocalizedNameDictionary[hero.Id] = hero.LocalizedName;
            HeroInternalNameDictionary[hero.Id] = hero.Name.Replace("npc_dota_hero_", "").ToLower(); // Convert to code name
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

    public static string GetHeroLocalizedName(int heroId)
    {
        if (!HeroLocalizedNameDictionary.Any()) InitializeAsync().Wait();

        return HeroLocalizedNameDictionary.TryGetValue(heroId, out var heroName) ? heroName : "Unknown Hero";
    }

    public static string GetHeroName(int heroId)
    {
        if (!HeroInternalNameDictionary.Any()) InitializeAsync().Wait();

        return HeroInternalNameDictionary.TryGetValue(heroId, out var heroName) ? heroName : "Unknown Hero";
    }

    public static string GetHeroImage(string heroName)
    {
        return ApiConstants.GetHeroImageUrl(heroName);
    }

    public static string GetHeroImage(int heroId)
    {
        return ApiConstants.GetHeroImageUrl(GetHeroName(heroId));
    }
}