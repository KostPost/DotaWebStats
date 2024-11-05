using System.Text.Json.Serialization;

namespace DotaWebStats.Models.DotaData;

public class WinLoseStats
{
    [JsonPropertyName("win")]
    public int Win { get; set; }
    
    [JsonPropertyName("lose")]
    public int Lose { get; set; }
    public double WinRate { get; set; }
}