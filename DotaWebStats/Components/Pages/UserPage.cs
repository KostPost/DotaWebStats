using Microsoft.AspNetCore.Components;
using ModelClasses;
using Services;
using Services.Constants;

namespace DotaWebStats.Components.Pages;

public partial class UserPage
{
    [Parameter] public string Id { get; set; }
    public long Dota2Id { get; set; }
    public long SteamId { get; set; }

    private SteamUserData.Player? _player;
    private bool _isLoading = true;
    private string? _errorMessage;

    [Inject] private ISteamDataService SteamDataService { get; set; } = null!;
    [Inject] private ILogger<UserPage> Logger { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        IdDifference();
        await LoadUserData();
    }

    private void IdDifference()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Console.WriteLine("Error: ID is null or empty.");
            return;
        }

        try
        {
            Console.WriteLine($"ID.Length = {Id.Length}");
            if (Id.Length == 9)
            {
                if (long.TryParse(Id, out long dota2Id))
                {
                    Dota2Id = dota2Id;
                    SteamId = NumConstats.SteamIdToDota2IdDiff + Dota2Id;
                }
                else
                {
                    Console.WriteLine("Error: The Dota 2 ID could not be parsed.");
                }
            }
            else if (Id.Length == 17)
            {
                if (long.TryParse(Id, out long steamId))
                {
                    SteamId = steamId;
                    Dota2Id = SteamId - NumConstats.SteamIdToDota2IdDiff;
                }
                else
                {
                    Console.WriteLine("Error: The Steam ID could not be parsed.");
                }
            }
            else
            {
                Console.WriteLine("Error: ID length is not recognized. Expected length is 9 or 16.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ID difference calculation error: {ex.Message}");
        }
    }

    private async Task LoadUserData()
    {
        _isLoading = true;
        _errorMessage = null;
        try
        {
            _player = await SteamDataService.GetPlayerSummary(SteamId);

            if (_player != null)
            {
                Console.WriteLine($"Player loaded: {_player.PersonaName}");
            }
            else
            {
                _errorMessage = "Player data not found.";
            }
        }

        catch (Exception ex)
        {
            Logger.LogError(ex, $"Failed to load player data for Dota2ID: {SteamId}");
            _errorMessage = $"Failed to load player data: {ex.Message}";
        }

        finally
        {
            _isLoading = false;
        }
    }
}