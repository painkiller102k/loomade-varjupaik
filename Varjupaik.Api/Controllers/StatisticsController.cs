using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

/// <summary>
/// Statistika
/// </summary>
[ApiController]
[Route("api/statistics")]
[Produces("application/json")]
public class StatisticsController : ControllerBase
{
    private readonly StatisticsService _statisticsService;

    public StatisticsController(StatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Keskmine aeg varjupaigas liikide kaupa (avalik)
    /// </summary>
    /// <remarks>
    /// Adopteeritud loomadel arvestatakse aega lisamisest adopteerimiseni, teistel lisamisest tänaseni.
    /// </remarks>
    /// <response code="200">Iga liigi keskmine päevade arv</response>
    [HttpGet("average-stay")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<AverageStayDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAverageStay()
    {
        var result = await _statisticsService.GetAverageStayAsync();
        return Ok(result);
    }
}
