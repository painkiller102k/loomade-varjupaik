using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Exceptions;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly ILogger<AuthService> _logger;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(IUserRepository users, ILogger<AuthService> logger)
    {
        _users = users;
        _logger = logger;
    }

    // Uus kasutaja saab alati rolli Visitor
    public async Task<User> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (await _users.EmailExistsAsync(email))
        {
            _logger.LogWarning("Registreerimine ebaõnnestus, email on juba kasutusel: {Email}", email);
            throw new BadRequestException("Selle emailiga kasutaja on juba olemas");
        }

        var user = new User
        {
            Email = email,
            Name = dto.Name,
            Role = Roles.Visitor
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        await _users.AddAsync(user);
        _logger.LogInformation("Uus kasutaja registreeriti: {Email}", email);
        return user;
    }

    // Tagastab kasutaja kui email ja parool on õiged, muidu viskab UnauthorizedException
    public async Task<User> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var user = await _users.GetByEmailAsync(email);

        var ok = user != null
            && _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) != PasswordVerificationResult.Failed;
        if (!ok)
        {
            _logger.LogWarning("Ebaõnnestunud sisselogimine: {Email}", email);
            throw new UnauthorizedException("Vale email või parool");
        }

        _logger.LogInformation("Kasutaja logis sisse: {Email} ({Role})", user!.Email, user.Role);
        return user;
    }
}
