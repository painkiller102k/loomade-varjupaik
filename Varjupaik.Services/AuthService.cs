using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;

namespace Varjupaik.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    // Uus kasutaja saab alati rolli Visitor
    public async Task<User?> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (await _db.Users.AnyAsync(u => u.Email == email))
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

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    // Tagastab kasutaja kui email ja parool on õiged, muidu null
    public async Task<User?> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        return result == PasswordVerificationResult.Failed ? null : user;
    }
}
