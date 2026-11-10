using System.ComponentModel.DataAnnotations;

namespace Varjupaik.Services.Dtos;

public class RegisterDto
{
    /// <example>mari@test.ee</example>
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    /// <example>Mari Maasikas</example>
    [Required]
    public string Name { get; set; } = "";

    /// <summary>Vähemalt 6 märki</summary>
    /// <example>mari123</example>
    [Required, MinLength(6)]
    public string Password { get; set; } = "";
}

public class LoginDto
{
    /// <example>staff@varjupaik.ee</example>
    [Required]
    public string Email { get; set; } = "";

    /// <example>Staff123!</example>
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
