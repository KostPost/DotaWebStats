using System.Text.Json.Serialization;

namespace DotaWebStats.Models.Common_Classes;

public class DraftTiming
{
    [JsonPropertyName("order")] public int Order { get; set; }

    [JsonPropertyName("pick")] public bool Pick { get; set; }

    [JsonPropertyName("active_team")] public int ActiveTeam { get; set; }

    [JsonPropertyName("hero_id")] public int HeroId { get; set; }

    [JsonPropertyName("player_slot")] public int PlayerSlot { get; set; }

    [JsonPropertyName("extra_time")] public int ExtraTime { get; set; }

    [JsonPropertyName("total_time_taken")] public int TotalTimeTaken { get; set; }
}

