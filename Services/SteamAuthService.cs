using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Services.Constants;

namespace Services
{
    public interface ISteamAuthService
    {
        Task InitializeAsync();
        void InitiateSteamAuth();
        Task LogoutAsync();
        long? SteamId { get; }
    }

    public class SteamAuthService : ISteamAuthService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        // private readonly IConfiguration _configuration;
        private const string RedirectUri = UrlConstants.IndexPage;

        public long? SteamId { get; private set; }
        private long? Dota2Id { get;  set; }

        public SteamAuthService(NavigationManager navigationManager, IJSRuntime jsRuntime)
        {
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            // _configuration = configuration;
        }

        private long? ConvertSteamIdToDota2Id(long steamId)
        {
            return steamId - NumConstats.SteamIdToDota2IdDiff;
        }

        public async Task InitializeAsync()
        {
            var uri = new Uri(_navigationManager.Uri);
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var steamIdParam = queryParams["openid.claimed_id"];

            if (!string.IsNullOrEmpty(steamIdParam))
            {
                var steamIdString = steamIdParam.Split('/').Last();
                if (long.TryParse(steamIdString, out long steamId))
                {
                    SteamId = steamId;
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "steamId", steamId.ToString());
                }
            }
            else
            {
                var storedSteamId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "steamId");
                if (long.TryParse(storedSteamId, out long steamId))
                {
                    SteamId = steamId;
                }
            }

            if (SteamId.HasValue)
            {
                Dota2Id = ConvertSteamIdToDota2Id(SteamId.Value);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "dota2Id", Dota2Id.ToString());
            }
            else
            {
                var storedDota2Id = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "dota2Id");
                if (long.TryParse(storedDota2Id, out long dota2Id))
                {
                    Dota2Id = dota2Id;
                }
            }
        }

        public void InitiateSteamAuth()
        {
            Console.WriteLine("Initiating Steam authentication...");
            var steamOpenIdUrl = "https://steamcommunity.com/openid/login";
            var parameters = new Dictionary<string, string>
            {
                { "openid.ns", "http://specs.openid.net/auth/2.0" },
                { "openid.mode", "checkid_setup" },
                { "openid.return_to", RedirectUri },
                { "openid.realm", "https://localhost:1234" },
                { "openid.identity", "http://specs.openid.net/auth/2.0/identifier_select" },
                { "openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select" }
            };

            var url =
                $"{steamOpenIdUrl}?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";
            _navigationManager.NavigateTo(url, forceLoad: true);
        }

        public async Task LogoutAsync()
        {
            SteamId = null;
            Dota2Id = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "steamId");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "dota2Id");
        }
    }
}