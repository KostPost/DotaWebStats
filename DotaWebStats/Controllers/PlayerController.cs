using DotaWebStats.Models;
using Microsoft.AspNetCore.Mvc;
using DotaWebStats.Services; // Include your services namespace

namespace DotaWebStats.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IDotaDataService _dotaDataService;

        public PlayerController(IDotaDataService dotaDataService)
        {
            _dotaDataService = dotaDataService;
        }

        [HttpGet("player/{id}")]
        public async Task<IActionResult> Index(long id) 
        {
            var userStats = await _dotaDataService.GetPlayerSummary(id);

            if (userStats == null)
            {
                return NotFound(); 
            }

            return View("~/Views/PlayersProfile/Player.cshtml", userStats);
        }
    }
}