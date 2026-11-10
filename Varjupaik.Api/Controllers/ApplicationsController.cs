using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly ApplicationService _applicationService;

    public ApplicationsController(ApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    // POST /api/animals/{id}/applications
    [HttpPost("api/animals/{id}/applications")]
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

        return Ok(application);
    }
}
