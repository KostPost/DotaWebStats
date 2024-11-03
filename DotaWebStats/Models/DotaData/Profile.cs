using System.Text.Json.Serialization;

namespace DotaWebStats.Models;

public class Profile
{
    [JsonPropertyName("personaname")] public string? UserName { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("plus")] public bool? Plus { get; set; }

    [JsonPropertyName("cheese")] public int? Cheese { get; set; }

    [JsonPropertyName("steamid")] public string? Steamid { get; set; }

    [JsonPropertyName("avatar")] public string? Avatar { get; set; }

    [JsonPropertyName("avatarmedium")] public string? AvatarMedium { get; set; }

    [JsonPropertyName("avatarfull")] public string? AvatarFull { get; set; }

    [JsonPropertyName("profileurl")] public string? Profileurl { get; set; }

    [JsonPropertyName("last_login")] public DateTime? LastLogin { get; set; }

    [JsonPropertyName("loccountrycode")] public string? LocCountryCode { get; set; }

    [JsonPropertyName("status")] public string? Status { get; set; }

    [JsonPropertyName("fh_unavailable")] public bool? FhUnavailable { get; set; }

    [JsonPropertyName("is_contributor")] public bool? IsContributor { get; set; }

    [JsonPropertyName("is_subscriber")] public bool? IsSubscriber { get; set; }
}