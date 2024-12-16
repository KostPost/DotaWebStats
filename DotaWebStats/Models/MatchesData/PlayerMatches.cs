using System.Text.Json.Serialization;
using DotaWebStats.Models.Common_Classes;

namespace DotaWebStats.Models.MatchesData
{
    public class PlayerMatches
    {
        [JsonPropertyName("match_id")]
        public long? MatchId { get; set; }  // Nullable long

        [JsonPropertyName("player_slot")]
        public int? PlayerSlot { get; set; }  // Nullable int

        [JsonPropertyName("radiant_win")]
        public bool? RadiantWin { get; set; }  // Nullable bool

        [JsonPropertyName("duration")]
        public int? Duration { get; set; }  // Nullable int

        [JsonPropertyName("game_mode")]
        public int? GameMode { get; set; }  // Nullable int

        [JsonPropertyName("lobby_type")]
        public int? LobbyType { get; set; }  // Nullable int

        [JsonPropertyName("hero_id")]
        public int? HeroId { get; set; }  // Nullable int

        [JsonPropertyName("start_time")]
        public long? StartTime { get; set; }  // Nullable long

        [JsonPropertyName("version")]
        public int? Version { get; set; }  // Nullable int

        [JsonPropertyName("kills")]
        public int? Kills { get; set; }  // Nullable int

        [JsonPropertyName("deaths")]
        public int? Deaths { get; set; }  // Nullable int

        [JsonPropertyName("assists")]
        public int? Assists { get; set; }  // Nullable int

        [JsonPropertyName("skill")]
        public int? Skill { get; set; }  // Nullable int

        [JsonPropertyName("average_rank")]
        public int? AverageRank { get; set; }  // Nullable int

        [JsonPropertyName("leaver_status")]
        public int? LeaverStatus { get; set; }  // Nullable int

        [JsonPropertyName("party_size")]
        public int? PartySize { get; set; }  // Nullable int

        [JsonPropertyName("hero_variant")]
        public int? HeroVariant { get; set; }  // Nullable int
    }
}