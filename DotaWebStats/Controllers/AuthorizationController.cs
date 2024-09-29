using DotaWebStats.Services;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace DotaWebStats.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly ISteamAuthService _authService;
        private readonly IDotaDataService _dotaDataService;

        public AuthorizationController(ISteamAuthService authService, IDotaDataService dotaDataService)
        {
            _authService = authService;
            _dotaDataService = dotaDataService;
        }

        public IActionResult Login()
        {
            return Redirect(_authService.GetSteamLoginUrl());
        }

        public async Task<IActionResult> Callback()
        {
            await _authService.ProcessCallbackAsync(HttpContext);
            
            if (_authService.Dota2Id.HasValue)
            {
                var userDotaStats = await _dotaDataService.GetPlayerSummary(_authService.Dota2Id.Value);
                if (userDotaStats != null)
                {
                    await _authService.SetUserDataAsync(userDotaStats, HttpContext);
                }
            }
            
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync(HttpContext);
            return RedirectToAction("Index", "Home");
        }
    }
}