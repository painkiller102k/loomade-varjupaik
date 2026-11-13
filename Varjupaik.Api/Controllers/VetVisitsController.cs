using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

/// <summary>
/// Looma veterinaarivisiidid (ainult töötajatele)
/// </summary>
[ApiController]
[Route("api/animals/{id}/vet-visits")]
[Authorize(Roles = "Admin,Staff")]
[Produces("application/json")]
public class VetVisitsController : ControllerBase
{
    private readonly VetVisitService _vetVisitService;

    public VetVisitsController(VetVisitService vetVisitService)
    {
        _vetVisitService = vetVisitService;
    }

    /// <summary>
    /// Looma veterinaarivisiitide ajalugu, uuemad enne
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <response code="200">Visiitide nimekiri</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Staff ega Admin</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<VetVisitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int id)
    {
        var visits = await _vetVisitService.GetHistoryAsync(id);
        return Ok(visits);
    }

    /// <summary>
    /// Lisab loomale veterinaarivisiidi
    /// </summary>
    /// <param name="id">Looma id</param>
    /// <param name="dto">Visiidi andmed</param>
    /// <response code="201">Visiit lisati</response>
    /// <response code="400">Vigased andmed</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Staff ega Admin</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpPost]
    [ProducesResponseType(typeof(VetVisitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(int id, VetVisitCreateDto dto)
    {
        var visit = await _vetVisitService.AddAsync(id, dto);
        return CreatedAtAction(nameof(GetHistory), new { id }, visit);
    }
}
