using DotaWebStats.Models;
using Microsoft.AspNetCore.Mvc;
using DotaWebStats.Services;
using DotaWebStats.Services.Helpers;

namespace DotaWebStats.Controllers
{
    public class PlayerController(IDotaDataService dotaDataService, ILogger<PlayerController> logger) : Controller
    {
        private readonly IDotaDataService _dotaDataService = dotaDataService ??
                                                             throw new ArgumentNullException(nameof(dotaDataService));

        private readonly ILogger<PlayerController> _logger = logger ??
                                                             throw new ArgumentNullException(nameof(logger));

        [HttpGet("player/{id}")]
        public Task<IActionResult> Index(long id) => GetPlayerData(id);


        [HttpGet("player/{id}/overview")]
        public Task<IActionResult> Overview(long id) => GetPlayerData(id);


        [HttpGet("player/{id}/matches")]
        public async Task<IActionResult> Matches(long id)
        {
            if (DotaDataHelper.IsDotaIdCorrect(id) == 0)
            {
                _logger.LogWarning("Invalid Dota ID format: {Id}", id);
                return BadRequest($"Invalid Dota ID format: {id}");
            }
        
            var matchesData = await _dotaDataService.GetPlayerMatchAsync(id);
        
            if (matchesData == null || !matchesData.Any())
            {
                _logger.LogInformation("No matches found for Dota ID: {Id}", id);
                return NotFound("No matches found for this player.");
            }
            
        
            ViewData["PlayerId"] = id;
            return View("~/Views/PlayersProfile/Matches.cshtml", matchesData);
        }




        private async Task<IActionResult> GetPlayerData(long id)
        {
            try
            {
                if (DotaDataHelper.IsDotaIdCorrect(id) == 0)
                {
                    _logger.LogWarning("Invalid Dota ID format: {Id}", id);
                    return BadRequest($"Invalid Dota ID format: {id}");
                }

                var playerData = await GetPlayerViewModel(id);
                if (!playerData.IsSuccess)
                {
                    return NotFound(playerData.ErrorMessage);
                }

                return View("~/Views/PlayersProfile/PlayerOverviewPage.cshtml", playerData.ViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request for player {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private async Task<(bool IsSuccess, PlayerViewModel? ViewModel, string? ErrorMessage)> GetPlayerViewModel(long id)
        {
            var userStats = await _dotaDataService.GetPlayerSummary(id);
            if (userStats == null)
            {
                _logger.LogInformation("Player summary not found for ID: {Id}", id);
                return (false, null, $"Player with ID {id} not found.");
            }

            var recentMatchesSummary = await _dotaDataService.RecentMatchesSummary(id);
            if (recentMatchesSummary == null)
            {
                _logger.LogInformation("Recent matches summary not found for ID: {Id}", id);
                return (false, null, $"Recent matches summary for player with ID {id} not found.");
            }

            var recentMatches = await _dotaDataService.GetRecentMatches(id);
            if (recentMatches == null)
            {
                _logger.LogInformation("Recent matches not found for ID: {Id}", id);
                return (false, null, $"Recent matches for player with ID {id} not found.");
            }

            var viewModel = new PlayerViewModel
            {
                UserStats = userStats,
                RecentMatchesSummary = recentMatchesSummary,
                RecentMatches = recentMatches
            };

            return (true, viewModel, null);
        }

    }
}
    
