using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

/// <summary>
/// Teavitused töötajatele
/// </summary>
[ApiController]
[Route("api/notifications")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationsController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Loob teavituse ja saadab selle töötajatele (ainult taustateenus, roll Service)
    /// </summary>
    /// <remarks>
    /// Teavitus salvestatakse andmebaasi ja saadetakse kohe ka kõigile ühendatud töötajatele WebSocketi kaudu.
    /// </remarks>
    /// <param name="dto">Pealkiri ja tekst</param>
    /// <response code="201">Teavitus loodi</response>
    /// <response code="400">Vigased andmed</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Service</response>
    [HttpPost]
    [Authorize(Roles = "Service")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(NotificationCreateDto dto)
    {
        var notification = await _notificationService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created, notification);
    }

    /// <summary>
    /// Viimased 20 teavitust, uuemad enne (Staff, Admin)
    /// </summary>
    /// <response code="200">Teavituste nimekiri</response>
    /// <response code="401">Pole sisse logitud</response>
    /// <response code="403">Roll ei ole Staff ega Admin</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(List<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetLatest()
    {
        var notifications = await _notificationService.GetLatestAsync();
        return Ok(notifications);
    }
}
