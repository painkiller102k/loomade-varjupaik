using Microsoft.AspNetCore.Identity;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(IUserRepository users)
    {
        _users = users;
    }

    // Uus kasutaja saab alati rolli Visitor
    public async Task<User?> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (await _users.EmailExistsAsync(email))
        {
            return null;
        }

        var user = new User
        {
            Email = email,
            Name = dto.Name,
            Role = Roles.Visitor
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        await _users.AddAsync(user);
        return user;
    }

    // Tagastab kasutaja kui email ja parool on õiged, muidu null
    public async Task<User?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var user = await _users.GetByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }
}
