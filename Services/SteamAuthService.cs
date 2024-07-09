using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;


namespace Services
{
    public interface ISteamAuthService
    {
        Task InitializeAsync();
        void InitiateSteamAuth();
        Task LogoutAsync();
        string SteamId { get; }
    }

    public class SteamAuthService : ISteamAuthService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly IConfiguration _configuration;
        private const string RedirectUri = "https://localhost:1234/";
            
        public string SteamId { get; private set; }
        private string ApiKey;
        public string Dota2Id { get; private set; }
        public SteamAuthService(NavigationManager navigationManager, IJSRuntime jsRuntime, IConfiguration configuration)
        {
            _navigationManager = navigationManager;
            _jsRuntime = jsRuntime;
            _configuration = configuration;
        }
        
        private string ConvertSteamIdToDota2Id(string steamId)
        {
            if (string.IsNullOrEmpty(steamId) || !steamId.All(char.IsDigit))
            {
                return null;
            }

            // Convert SteamId to long
            if (!long.TryParse(steamId, out long steamId64))
            {
                return null;
            }

            // The magic number is the difference between Steam ID and Dota 2 ID
            const long steamIdToDota2IdDiff = 76561197960265728;
            long dota2Id = steamId64 - steamIdToDota2IdDiff;

            return dota2Id.ToString();
        }

        public async Task InitializeAsync()
        {
            ApiKey = _configuration["Steam:ApiKey"];
            var uri = new Uri(_navigationManager.Uri);
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var steamIdParam = queryParams["openid.claimed_id"];

            if (!string.IsNullOrEmpty(steamIdParam))
            {
                SteamId = steamIdParam.Split('/').Last();
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "steamId", SteamId);
            }
            else
            {
                SteamId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "steamId");
            }

            if (!string.IsNullOrEmpty(SteamId))
            {
                Dota2Id = ConvertSteamIdToDota2Id(SteamId);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "dota2Id", Dota2Id);
            }
            else
            {
                Dota2Id = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "dota2Id");
            }
        }

        public void InitiateSteamAuth()
        {
            Console.WriteLine("Initiating Steam authentication...");
            var steamOpenIdUrl = "https://steamcommunity.com/openid/login";
            var parameters = new Dictionary<string, string>
            {
                {"openid.ns", "http://specs.openid.net/auth/2.0"},
                {"openid.mode", "checkid_setup"},
                {"openid.return_to", RedirectUri},
                {"openid.realm", "https://localhost:1234"},
                {"openid.identity", "http://specs.openid.net/auth/2.0/identifier_select"},
                {"openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select"}
            };

            var url = $"{steamOpenIdUrl}?{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))}";
            _navigationManager.NavigateTo(url, forceLoad: true);
        }

        public async Task LogoutAsync()
        {
            SteamId = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "steamId");
        }
    }
}
