using System.Text.Json;
using DotaWebStats.Constants;
using DotaWebStats.Models.DotaData;

namespace DotaWebStats.Services.Helpers;

public class DotaItemHelper
{
    private static readonly Dictionary<int, string> ItemLocalizedNameDictionary = new();
    
    private static readonly Dictionary<int, string> ItemInternalNameDictionary = new();
    
    private static readonly Lazy<Task> InitializeTask = new Lazy<Task>(InitializeAsyncInternal);

    public static Task InitializeAsync() => InitializeTask.Value;

    private static async Task InitializeAsyncInternal()
    {
        if (ItemLocalizedNameDictionary.Any()) return;

        var itemsDictionary = await FetchItemsFromApiAsync();
        if (itemsDictionary == null || !itemsDictionary.Any()) return;

        foreach (var item in itemsDictionary.Values)
        {
            if (item.Id > 0) 
            {
                ItemLocalizedNameDictionary[item.Id] = item.DisplayName;

                ItemInternalNameDictionary[item.Id] = item.ImageName;
            }
        }
    }

    private static async Task<Dictionary<string, DotaItem>?> FetchItemsFromApiAsync()
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetStringAsync(ApiConstants.DotaApi.GetItems());
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var itemsDictionary = JsonSerializer.Deserialize<Dictionary<string, DotaItem>>(response, options);
            return itemsDictionary;
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");
            // Optionally log the raw response for debugging
            Console.WriteLine("Raw JSON response: ");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching items: {ex.Message}");
            return null;
        }
    }



    public static string GetItemLocalizedName(int itemId)
    {
        if (!ItemLocalizedNameDictionary.Any()) InitializeAsync().Wait();

        return ItemLocalizedNameDictionary.TryGetValue(itemId, out var itemName) ? itemName : "Unknown Item";
    }
    
    private static string GetItemName(int itemId)
    {
        if (itemId <= 0) return string.Empty;
    
        if (!ItemLocalizedNameDictionary.Any()) 
            InitializeAsync().Wait();
        
        return ItemInternalNameDictionary.TryGetValue(itemId, out var itemName) 
            ? itemName 
            : "Unknown Item";
    }
    
    public static string GetItemImage(string itemName)
    {
        return ApiConstants.GetItemImageUrl(itemName);
    }
    
    
    
    
    public static string GetItemImage(int itemId)
    {
        if (itemId <= 0) return string.Empty;
    
        var itemName = GetItemName(itemId);
        return string.IsNullOrEmpty(itemName) || itemName == "Unknown Item" 
            ? string.Empty 
            : ApiConstants.GetItemImageUrl(itemName);
    }

}