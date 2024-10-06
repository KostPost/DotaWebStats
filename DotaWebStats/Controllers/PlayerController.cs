using DotaWebStats.Models;
using Microsoft.AspNetCore.Mvc;
using DotaWebStats.Services; // Include your services namespace

namespace DotaWebStats.Controllers
{
    public class PlayerController(IDotaDataService dotaDataService) : Controller
    {
        private readonly IDotaDataService _dotaDataService = dotaDataService;

        [HttpGet("player/{id}")]
        public async Task<IActionResult> Index(long id)
        {
            var userStats = await _dotaDataService.GetPlayerSummary(id);
    
            if (userStats == null)
            {
                return NotFound($"Player with ID {id} not found.");
            }

            var recentMatches = await _dotaDataService.GetRecentMatches(id);
    
            if (recentMatches == null)
            {
                return NotFound($"Recent matches for player with ID {id} not found.");
            }

            var viewModel = new PlayerViewModel
            {
                UserStats = userStats,
                RecentMatches = recentMatches // Adjust according to the actual type returned
            };

            return View("~/Views/PlayersProfile/Player.cshtml", viewModel);
        }



        
        
        
        
        [HttpGet("player/{id}/overview")]
        public async Task<IActionResult> Overview(long id)
        {
            var userStats = await _dotaDataService.GetPlayerSummary(id);
    
            if (userStats == null)
            {
                return NotFound($"Player with ID {id} not found.");
            }

            var recentMatches = await _dotaDataService.GetRecentMatches(id);
    
            if (recentMatches == null)
            {
                return NotFound($"Recent matches for player with ID {id} not found.");
            }

            var viewModel = new PlayerViewModel
            {
                UserStats = userStats,
                RecentMatches = recentMatches 
            };

            return View("~/Views/PlayersProfile/Player.cshtml", viewModel);
        }
    }
}