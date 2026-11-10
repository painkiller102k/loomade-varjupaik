using Microsoft.AspNetCore.Identity;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Data;

public static class DbSeeder
{
    // Loob algsed kasutajad (Admin, Staff, Service), kui neid veel pole
    public static void SeedUsers(AppDbContext db, List<SeedUser> users)
    {
        var hasher = new PasswordHasher<User>();

        foreach (var seed in users)
        {
            var email = seed.Email.ToLower();
            if (db.Users.Any(u => u.Email == email))
            {
                continue;
            }

            var user = new User
            {
                Email = email,
                Name = seed.Name,
                Role = seed.Role
            };
            user.PasswordHash = hasher.HashPassword(user, seed.Password);
            db.Users.Add(user);
        }

        db.SaveChanges();
    }
}

public class SeedUser
{
    public string Email { get; set; } = "";
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "";
}
