using System.ComponentModel.DataAnnotations;

namespace Varjupaik.Services.Dtos;

public class RegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Name { get; set; } = "";

    [Required, MinLength(6)]
    public string Password { get; set; } = "";
}

public class LoginDto
{
    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

public class LoginResultDto
{
    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}
