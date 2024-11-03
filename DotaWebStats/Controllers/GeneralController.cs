using DotaWebStats.Models.MatchesData;
using DotaWebStats.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotaWebStats.Controllers;

public class GeneralController(IDotaDataService dotaDataService) : Controller
{
    private readonly IDotaDataService _dotaDataService = dotaDataService;

  

    [HttpGet("match/{id}")]
    public async Task<IActionResult> MatchInfo(long id)
    {
        var match = await dotaDataService.GetMatchInfo(id);
        if (match == null)
        {
            return NotFound("Match not found.");
        }

        return View("~/Views/GeneralPages/Match.cshtml", match);
    }
}