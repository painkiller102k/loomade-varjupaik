using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Api.Auth;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

/// <summary>
/// Registreerimine ja sisselogimine
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public AuthController(AuthService authService, TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Registreerib uue külastaja (roll Visitor) ja tagastab tokeni
    /// </summary>
    /// <response code="200">Kasutaja loodi</response>
    /// <response code="400">Email on juba kasutusel või andmed on vigased</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);

        return Ok(new LoginResultDto
        {
            Token = _tokenService.CreateToken(user),
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        });
    }

    /// <summary>
    /// Sisselogimine, tagastab JWT tokeni
    /// </summary>
    /// <response code="200">Õige email ja parool</response>
    /// <response code="401">Vale email või parool</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _authService.LoginAsync(dto);

        return Ok(new LoginResultDto
        {
            Token = _tokenService.CreateToken(user),
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        });
    }
}
