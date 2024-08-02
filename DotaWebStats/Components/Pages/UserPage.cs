using Microsoft.AspNetCore.Components;
using ModelClasses;
using Services;

namespace DotaWebStats.Components.Pages;

public partial class UserPage
{
    [Parameter] public long Dota2Id { get; set; } 
    public 

    private SteamUserData.Player? _player;
    private bool _isLoading = true;
    private string? _errorMessage;

    [Inject] private ISteamDataService SteamDataService { get; set; } = null!;
    [Inject] private ILogger<UserPage> Logger { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserData();
    }

    private string? _previousDota2Id;
    

    private async Task LoadUserData()
    {
        
        
        _isLoading = true;
        _errorMessage = null;
        try
        {
            if (!string.IsNullOrEmpty(Dota2Id))
            {
                _player = await SteamDataService.GetPlayerSummary(Dota2Id);

                if (_player != null)
                {
                    Console.WriteLine($"Player loaded: {_player.PersonaName}");
                }
                else
                {
                    _errorMessage = "Player data not found.";
                }
            }
            else
            {
                _errorMessage = "Invalid Dota 2 ID.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to load player data for Dota2ID: {Dota2Id}");
            _errorMessage = $"Failed to load player data: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }
}