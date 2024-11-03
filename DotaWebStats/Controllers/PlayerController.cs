using DotaWebStats.Models;
using Microsoft.AspNetCore.Mvc;
using DotaWebStats.Services;

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

            var recentMatchesSummary = await _dotaDataService.RecentMatchesSummary(id);

            if (recentMatchesSummary == null)
            {
                return NotFound($"Recent matches for player with ID {id} not found.");
            }

            var recentMatches = await _dotaDataService.GetRecentMatches(id);
            if (recentMatches == null)
            {
                return NotFound($"Recent matches for player with ID {id} not found.");
            }


            var viewModel = new PlayerViewModel
            {
                UserStats = userStats,
                RecentMatchesSummary = recentMatchesSummary,
                RecentMatches = recentMatches
            };

            return View("~/Views/PlayersProfile/Player.cshtml", viewModel);
        }





        [HttpGet("player/{id}/overview")]
        public async Task<IActionResult> Overview(long id)
        {
            var userStats = await _dotaDataService.GetPlayerSummary(id);

            if (userStats is null) // Using is null for clarity with nullable reference types
            {
                return NotFound($"Player with ID {id} not found.");
            }

            var recentMatchesSummary = await _dotaDataService.RecentMatchesSummary(id);

            if (recentMatchesSummary is null)
            {
                return NotFound($"Recent matches for player with ID {id} not found.");
            }

            var recentMatches = await _dotaDataService.GetRecentMatches(id);
            if (recentMatches is null)
            {
                return NotFound($"Recent matches for player with ID {id} not found.");
            }

            var viewModel = new PlayerViewModel
            {
                UserStats = userStats,
                RecentMatchesSummary = recentMatchesSummary,
                RecentMatches = recentMatches
            };

            return View("~/Views/PlayersProfile/Player.cshtml", viewModel);
        }


      
        
    }
}