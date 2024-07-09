using Microsoft.AspNetCore.Components;
using ModelClasses;
using Services;
using System.Threading.Tasks;

namespace DotaWebStats.Components.Pages;

public partial class UserPage
{
    [Parameter] public string Dota2Id { get; set; }

    private SteamUserData.Player player;
    private bool isLoading = true;
    private string errorMessage;

    [Inject] private ISteamDataService SteamDataService { get; set; }
    [Inject] private ILogger<UserPage> Logger { get; set; }
    [Inject] private ISteamAuthService AuthService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await InitializeUserData();
    }

    protected override async Task OnParametersSetAsync()
    {
        await InitializeUserData();
    }

    private async Task InitializeUserData()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            await AuthService.InitializeAsync();

            var steamIdNullable = AuthService.SteamId;
            if (steamIdNullable.HasValue && steamIdNullable.Value != 0)
            {
                long steamId = steamIdNullable.Value;
                string steamIdString = steamId.ToString();

                player = await SteamDataService.GetPlayerSummary(steamIdString);

                if (player != null)
                {
                    Console.WriteLine($"Player loaded: {player.PersonaName}");
                }
                else
                {
                    errorMessage = "Player data not found.";
                }
            }
            else
            {
                errorMessage = "Steam ID is not available or is zero.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to initialize or load player data for Dota2ID: {Dota2Id}");
            errorMessage = $"Failed to load player data: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}