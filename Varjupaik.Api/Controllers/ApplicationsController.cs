using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;

namespace Varjupaik.Api.Controllers;

/// <summary>
/// Adopteerimise taotlused
/// </summary>
[ApiController]
[Produces("application/json")]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationService _applicationService;

    public ApplicationsController(ApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    /// <summary>
    /// Külastaja esitab loomale adopteerimise taotluse
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <param name="dto">Taotluse sõnum</param>
    /// <response code="201">Taotlus loodi, loom muutub olekusse Reserved</response>
    /// <response code="400">Loom on juba adopteeritud või taotlus on juba olemas</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Visitor</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpPost("api/animals/{id}/applications")]
    [Authorize(Roles = "Visitor")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(int id, ApplicationCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var application = await _applicationService.CreateAsync(id, userId, dto);
        return StatusCode(StatusCodes.Status201Created, application);
    }

    /// <summary>
    /// Kõik taotlused (Staff, Admin), valikuline filter oleku järgi
    /// </summary>
    /// <param name="status">New, Approved või Rejected</param>
    /// <response code="200">Taotluste nimekiri, uuemad enne</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Staff ega Admin</response>
    [HttpGet("api/applications")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(List<ApplicationListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll([FromQuery] ApplicationStatus? status)
    {
        var applications = await _applicationService.GetAllAsync(status);
        return Ok(applications);
    }

    /// <summary>
    /// Sisseloginud külastaja enda taotlused
    /// </summary>
    /// <response code="200">Enda taotluste nimekiri</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Visitor</response>
    [HttpGet("api/applications/my")]
    [Authorize(Roles = "Visitor")]
    [ProducesResponseType(typeof(List<ApplicationListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMy()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var applications = await _applicationService.GetMyAsync(userId);
        return Ok(applications);
    }

    /// <summary>
    /// Taotluse menetlemine: kinnitamine või tagasilükkamine
    /// </summary>
    /// <remarks>
    /// Looma olek muutub samas tehingus:
    /// kinnitamisel saab loom olekuks Adopted (teised uued taotlused lükatakse tagasi),
    /// tagasilükkamisel ja uusi taotlusi pole, muutub loom jälle Available.
    /// </remarks>
    /// <param name="id">Taotluse id</param>
    /// <param name="dto">Uus olek</param>
    /// <response code="200">Olek muudeti</response>
    /// <response code="400">Taotlus on juba menetletud või olek on vale</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Staff ega Admin</response>
    /// <response code="404">Taotlust ei leitud</response>
    [HttpPatch("api/applications/{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(ApplicationListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeStatus(int id, ApplicationStatusDto dto)
    {
        var application = await _applicationService.ChangeStatusAsync(id, dto.Status);
        return Ok(application);
    }
}
