using Microsoft.AspNetCore.Components;
using ModelClasses;
using Services;
using System.Threading.Tasks;
using System;

namespace DotaWebStats.Components.Pages;

public partial class UserPage
{
    [Parameter] public string Dota2Id { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private ILogger<UserPage> Logger { get; set; }
    [Inject] private ISteamDataService SteamDataService { get; set; }
    [Inject] private IDotaDataService DotaDataService { get; set; }

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

    private async Task LoadPlayerData()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            Logger.LogInformation($"Loading player data for ID: {Dota2Id}");

            // First, try to load the player data assuming Dota2Id is provided
            player = await SteamDataService.GetPlayerSummaryByDota2Id(Dota2Id);

            // If player is null, the ID might be a SteamId
            if (player == null)
            {
                string convertedDota2Id = SteamDataService.ConvertSteamIdToDota2Id(Dota2Id);
                if (!string.IsNullOrEmpty(convertedDota2Id))
                {
                    player = await SteamDataService.GetPlayerSummaryByDota2Id(convertedDota2Id);
                    if (player != null)
                    {
                        // Update the URL to use the correct Dota2Id
                        NavigationManager.NavigateTo($"/players/{convertedDota2Id}", replace: true);
                        return; // The page will reinitialize with the new URL
                    }
                }
            }

            if (player == null)
            {
                errorMessage = "Player not found. The provided ID doesn't correspond to an existing Dota 2 or Steam account.";
                return;
            }

            matchStats = await DotaDataService.GetMatchStats(Dota2Id);
            player.Rank = await DotaDataService.GetDotaRank(Dota2Id);

            Logger.LogInformation($"Successfully loaded data for player: {player.PersonaName}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to load player data for ID: {Dota2Id}");
            errorMessage = $"Failed to load player data: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}