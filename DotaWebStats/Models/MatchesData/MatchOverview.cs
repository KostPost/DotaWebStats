namespace DotaWebStats.Models.MatchesData;

using System.Collections.Generic;
using System.Text.Json.Serialization;

// for the first add a team victory, the score, time, game data, game type, match id,.. region

public class MatchOverview
{
    [JsonPropertyName("match_id")]
    public long MatchId { get; set; }

    [JsonPropertyName("barracks_status_dire")]
    public int BarracksStatusDire { get; set; }

    [JsonPropertyName("barracks_status_radiant")]
    public int BarracksStatusRadiant { get; set; }

    [JsonPropertyName("tower_status_dire")]
    public int TowerStatusDire { get; set; }

    [JsonPropertyName("tower_status_radiant")]
    public int TowerStatusRadiant { get; set; }

    [JsonPropertyName("dire_score")]
    public int DireScore { get; set; }

    [JsonPropertyName("radiant_score")]
    public int RadiantScore { get; set; }

    [JsonPropertyName("radiant_win")]
    public bool RadiantWin { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    [JsonPropertyName("replay_url")]
    public string ReplayUrl { get; set; }

    [JsonPropertyName("game_mode")]
    public int GameMode { get; set; }

    [JsonPropertyName("lobby_type")]
    public int LobbyType { get; set; }

    [JsonPropertyName("human_players")]
    public int HumanPlayers { get; set; }

    [JsonPropertyName("first_blood_time")]
    public int FirstBloodTime { get; set; }

    [JsonPropertyName("picks_bans")]
    public List<PickBan> PicksBans { get; set; }

    [JsonPropertyName("players")]
    public List<Player> Players { get; set; }

    [JsonPropertyName("patch")]
    public int Patch { get; set; }

    [JsonPropertyName("region")]
    public int Region { get; set; }
}

public class Player
{
    [JsonPropertyName("player_slot")]
    public int PlayerSlot { get; set; }

    [JsonPropertyName("account_id")]
    public long AccountId { get; set; }

    [JsonPropertyName("hero_id")]
    public int HeroId { get; set; }

    [JsonPropertyName("kills")]
    public int Kills { get; set; }

    [JsonPropertyName("deaths")]
    public int Deaths { get; set; }

    [JsonPropertyName("assists")]
    public int Assists { get; set; }

    [JsonPropertyName("hero_damage")]
    public int HeroDamage { get; set; }

    [JsonPropertyName("tower_damage")]
    public int TowerDamage { get; set; }

    [JsonPropertyName("hero_healing")]
    public int HeroHealing { get; set; }

    [JsonPropertyName("gold_per_min")]
    public int GoldPerMin { get; set; }

    [JsonPropertyName("xp_per_min")]
    public int XpPerMin { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("last_hits")]
    public int LastHits { get; set; }

    [JsonPropertyName("denies")]
    public int Denies { get; set; }

    [JsonPropertyName("total_gold")]
    public int TotalGold { get; set; }

    [JsonPropertyName("total_xp")]
    public int TotalXp { get; set; }

    [JsonPropertyName("item_0")]
    public int Item0 { get; set; }

    [JsonPropertyName("item_1")]
    public int Item1 { get; set; }

    [JsonPropertyName("item_2")]
    public int Item2 { get; set; }

    [JsonPropertyName("item_3")]
    public int Item3 { get; set; }

    [JsonPropertyName("item_4")]
    public int Item4 { get; set; }

    [JsonPropertyName("item_5")]
    public int Item5 { get; set; }

    [JsonPropertyName("backpack_0")]
    public int Backpack0 { get; set; }

    [JsonPropertyName("backpack_1")]
    public int Backpack1 { get; set; }

    [JsonPropertyName("backpack_2")]
    public int Backpack2 { get; set; }

    [JsonPropertyName("personaname")]
    public string PersonaName { get; set; }

    [JsonPropertyName("isRadiant")]
    public bool IsRadiant { get; set; }

    [JsonPropertyName("camps_stacked")]
    public int CampsStacked { get; set; }

    [JsonPropertyName("rune_pickups")]
    public int RunePickups { get; set; }

    [JsonPropertyName("stuns")]
    public decimal Stuns { get; set; }

    [JsonPropertyName("actions_per_min")]
    public int ActionsPerMin { get; set; }
    
    public string? HeroImageUrl { get; set; }

}

public class PickBan
{
    [JsonPropertyName("is_pick")]
    public bool IsPick { get; set; }

    [JsonPropertyName("hero_id")]
    public int HeroId { get; set; }

    [JsonPropertyName("team")]
    public int Team { get; set; }

    [JsonPropertyName("order")]
    public int Order { get; set; }
}