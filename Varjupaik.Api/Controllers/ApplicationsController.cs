using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

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
    /// <response code="201">Taotlus loodi</response>
    /// <response code="400">Loom on juba adopteeritud või taotlus on juba olemas</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Visitor</response>
    /// <response code="404">Looma ei leitud</response>
    [HttpPost("api/animals/{id}/applications")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Visitor")]
    public async Task<IActionResult> Create(int id, ApplicationCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var (application, error) = await _applicationService.CreateAsync(id, userId, dto);
        if (error == "Looma ei leitud")
        {
            return NotFound(new { message = error });
        }
        if (error != null)
        {
            return BadRequest(new { message = error });
        }

        return StatusCode(StatusCodes.Status201Created, application);
    }
}
