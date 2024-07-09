using Microsoft.AspNetCore.Components;
using ModelClasses;
using Services;
using System.Threading.Tasks;

namespace DotaWebStats.Components.Pages;

public partial class UserPage
{
    [Parameter] public string Dota2Id { get; set; }

    private SteamUserData.Player player;
    private MatchStats matchStats;
    private bool isLoading = true;
    private string errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadPlayerData();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadPlayerData();
    }

    [Inject] private ILogger<UserPage> Logger { get; set; }

    private async Task LoadPlayerData()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
      
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to load player data for Dota2ID: {Dota2Id}");
            errorMessage = $"Failed to load player data: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}