using System.Text.Json.Serialization;

namespace DotaWebStats.Models.Common_Classes;

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