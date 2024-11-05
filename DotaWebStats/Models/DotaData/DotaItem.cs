using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotaWebStats.Models.DotaData
{
    public class DotaItem
    {
        [JsonPropertyName("abilities")] public List<Ability>? Abilities { get; set; }

        [JsonPropertyName("hint")] public List<string>? Hint { get; set; }

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("img")] public string? ImageUrl { get; set; }
        
        public string? ImageName
        {
            get
            {
                if (ImageUrl == null)
                    return null;

                // Extract the item name from the image URL
                var segments = ImageUrl.Split('/');
                var lastSegment = segments.LastOrDefault();
                if (lastSegment != null)
                {
                    var nameWithExtension = lastSegment.Split('?')[0]; // Remove query string if present
                    var name = Path.GetFileNameWithoutExtension(nameWithExtension); // Remove the extension
                    return name; // Return the item name
                }

                return null; // Fallback in case of failure
            }
        }

        [JsonPropertyName("dname")] public string? DisplayName { get; set; }

        [JsonPropertyName("qual")] public string? Quality { get; set; }

        [JsonPropertyName("cost")] public int? Cost { get; set; }

        // Change behavior to a dynamic type to accommodate different formats
        [JsonPropertyName("behavior")] public object? Behavior { get; set; } // Using object to handle varied types

        [JsonPropertyName("notes")] public string? Notes { get; set; }

        [JsonPropertyName("attrib")] public List<Attribute>? Attributes { get; set; }


        [JsonPropertyName("lore")] public string? Lore { get; set; }

        [JsonPropertyName("components")] public List<string>? Components { get; set; }

        [JsonPropertyName("created")] public bool IsCreated { get; set; }


        [JsonPropertyName("mc")] public object? ManaCost { get; set; }

        [JsonPropertyName("hc")] public object? HealthCost { get; set; }

        [JsonPropertyName("cd")] public object? Cooldown { get; set; }

        [JsonPropertyName("charges")] public object HasCharges { get; set; }


        public DotaItem()
        {
            GetIntValue(Cooldown);

            GetIntValue(ManaCost);

            GetIntValue(HealthCost);
        }


        private int? GetIntValue(object? value)
        {
            switch (value)
            {
                case int intValue:
                    return intValue;
                case bool boolValue when !boolValue: // Check for false
                    return 0; // or return null based on your logic
                case string strValue when int.TryParse(strValue, out var parsedValue):
                    return parsedValue;
                default:
                    return null; // Handle other cases or return null
            }
        }
    }


    public class Ability
    {
        [JsonPropertyName("type")] public string? Type { get; set; }

        [JsonPropertyName("title")] public string? Title { get; set; }

        [JsonPropertyName("description")] public string? Description { get; set; }
    }

    public class Attribute
    {
        [JsonPropertyName("key")] public string? Key { get; set; }

        [JsonPropertyName("value")] public string? Value { get; set; }
    }
}