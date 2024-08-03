using ModelClasses;
using Services.Constants;

namespace DotaWebStats.Components.Layout;

public partial class MainLayout
{
    private SteamUserData.Player? SteamUserData { get; set; }
    private bool _showDropdown;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AuthService.InitializeAsync();
            var steamIdNullable = AuthService.SteamId;

            if (steamIdNullable.HasValue && steamIdNullable.Value != 0)
            {
                long steamId = steamIdNullable.Value;

                Console.WriteLine("MainLAyout - " + steamId);
                SteamUserData = await SteamDataService.GetPlayerSummary(steamId);
                StateHasChanged(); 
            }
            else
            {
                Console.WriteLine("Steam ID is not available or is zero.");
            }
        }
    }

    private void ToggleDropdown()
    {
        _showDropdown = !_showDropdown;
    }

    private void AuthenticationStart()
    {
        AuthService.InitiateSteamAuth();
    }

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        SteamUserData = null;
        NavigationManager.NavigateTo("/", forceLoad: true);
    }

    private void NavigateToProfile()
    {
        if (SteamUserData != null)
        {
            if (!string.IsNullOrEmpty(SteamUserData.Dota2Id))
            {
                NavigationManager.NavigateTo(UrlConstants.PlayerProfileUrl(SteamUserData.Dota2Id.ToString()));
            }
            else if (!string.IsNullOrEmpty(SteamUserData.Steamid))
            {
                NavigationManager.NavigateTo(UrlConstants.PlayerProfileUrl(SteamUserData.Steamid));
            }
            else
            {
                Console.WriteLine("Neither Dota2Id nor SteamId is available for navigation.");
            }
        }
        else
        {
            Console.WriteLine("User data is not available.");
        }
    }
}