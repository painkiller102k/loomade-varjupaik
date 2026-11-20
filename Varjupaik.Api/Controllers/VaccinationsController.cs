using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

/// <summary>
/// Vaktsineerimised: lähenevad tähtajad ja looma vaktsineerimiste ajalugu
/// </summary>
[ApiController]
[Produces("application/json")]
public class VaccinationsController : ControllerBase
{
    private readonly VaccinationService _vaccinationService;

    public VaccinationsController(VaccinationService vaccinationService)
    {
        _vaccinationService = vaccinationService;
    }

    /// <summary>
    /// Lähenevad vaktsineerimised (Staff, Admin ja taustateenus Service)
    /// </summary>
    /// <remarks>
    /// Tagastab vaktsineerimised, mille järgmine tähtaeg on järgmise <c>days</c> päeva jooksul või juba möödas.
    /// Adopteeritud loomi ei näidata. Kõige kiireloomulisemad on esimesed.
    /// </remarks>
    /// <param name="days">Mitu päeva ette vaadata (1 kuni 365)</param>
    /// <response code="200">Lähenevate vaktsineerimiste nimekiri</response>
    /// <response code="400">days ei ole vahemikus 1 kuni 365</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Staff, Admin ega Service</response>
    [HttpGet("api/vaccinations/upcoming")]
    [Authorize(Roles = "Admin,Staff,Service")]
    [ProducesResponseType(typeof(List<UpcomingVaccinationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUpcoming([FromQuery] int days = 7)
    {
        var vaccinations = await _vaccinationService.GetUpcomingAsync(days);
        return Ok(vaccinations);
    }

    /// <summary>
    /// Looma vaktsineerimiste ajalugu, uuemad enne
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <response code="200">Vaktsineerimiste nimekiri</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpGet("api/animals/{id}/vaccinations")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(List<VaccinationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByAnimal(int id)
    {
        var vaccinations = await _vaccinationService.GetByAnimalAsync(id);
        return Ok(vaccinations);
    }

    /// <summary>
    /// Lisab loomale vaktsineerimise
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <param name="dto">Vaktsineerimise andmed</param>
    /// <response code="201">Vaktsineerimine lisati</response>
    /// <response code="400">Vigased andmed</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpPost("api/animals/{id}/vaccinations")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(VaccinationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(int id, VaccinationCreateDto dto)
    {
        var vaccination = await _vaccinationService.AddAsync(id, dto);
        return CreatedAtAction(nameof(GetByAnimal), new { id }, vaccination);
    }
}
