using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DotaWebStats.Models;

public class DotaHero
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("localized_name")] public string LocalizedName { get; set; } = string.Empty;

    [JsonPropertyName("primary_attr")] public string? PrimaryAttr { get; set; }

    [JsonPropertyName("attack_type")] public string? AttackType { get; set; }

    [JsonPropertyName("roles")] public List<string> Roles { get; set; } = new List<string>();

    [JsonPropertyName("legs")] public int Legs { get; set; }
}