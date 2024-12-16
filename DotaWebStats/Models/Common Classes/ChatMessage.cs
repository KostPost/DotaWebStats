using System.Text.Json.Serialization;

namespace DotaWebStats.Models.Common_Classes;

public class ChatMessage
{
    [JsonPropertyName("time")] public int Time { get; set; }

    [JsonPropertyName("unit")] public string Unit { get; set; }

    [JsonPropertyName("key")] public string Key { get; set; }

    [JsonPropertyName("slot")] public int Slot { get; set; }

    [JsonPropertyName("player_slot")] public int PlayerSlot { get; set; }
}