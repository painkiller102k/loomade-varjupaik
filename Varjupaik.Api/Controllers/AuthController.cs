using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Varjupaik.Api.Auth;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;

namespace Varjupaik.Api.Controllers;

[ApiController]
[Route("api/auth")]
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

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        if (user == null)
        {
            return BadRequest(new { message = "Selle emailiga kasutaja on juba olemas" });
        }

        return Ok(new LoginResultDto
        {
            Token = _tokenService.CreateToken(user),
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _authService.LoginAsync(dto);
        if (user == null)
        {
            return Unauthorized(new { message = "Vale email või parool" });
        }

        return Ok(new LoginResultDto
        {
            Token = _tokenService.CreateToken(user),
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        });
    }
}
