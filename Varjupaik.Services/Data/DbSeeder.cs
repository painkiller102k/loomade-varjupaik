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

    // Algandmed: mõned loomad, et nimekiri poleks tühi
    public static void SeedAnimals(AppDbContext db)
    {
        if (db.Animals.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;
        db.Animals.AddRange(
            new Animal { Name = "Muri", Species = "Koer", Age = 3, Description = "Sõbralik ja rõõmsameelne koer, kes armastab pikki jalutuskäike.", Status = AnimalStatus.Available, CreatedAt = now.AddDays(-40) },
            new Animal { Name = "Miisu", Species = "Kass", Age = 2, Description = "Rahulik kass, kes naudib päikese käes peesitamist ja paitusi.", Status = AnimalStatus.Available, CreatedAt = now.AddDays(-12) },
            new Animal { Name = "Reks", Species = "Koer", Age = 6, Description = "Tark ja kuulekas saksa lambakoer, sobib aktiivsele perele.", Status = AnimalStatus.Reserved, CreatedAt = now.AddDays(-75) },
            new Animal { Name = "Nurr", Species = "Kass", Age = 5, Description = "Uudishimulik kass, saab hästi läbi teiste kassidega.", Status = AnimalStatus.Available, CreatedAt = now.AddDays(-30) },
            new Animal { Name = "Lumi", Species = "Küülik", Age = 1, Description = "Valge kohev küülik, armastab porgandeid ja salatit.", Status = AnimalStatus.Available, CreatedAt = now.AddDays(-5) },
            new Animal { Name = "Bella", Species = "Koer", Age = 1, Description = "Energiline kutsikas, kes õpib kiiresti uusi trikke.", Status = AnimalStatus.Available, CreatedAt = now.AddDays(-3) },
            new Animal { Name = "Tom", Species = "Kass", Age = 9, Description = "Vanem ja väga hell kass, otsib vaikset kodu.", Status = AnimalStatus.Available, CreatedAt = now.AddDays(-120) },
            new Animal { Name = "Pontu", Species = "Koer", Age = 4, Description = "Lõbus koer, kes armastab lapsi ja palliga mängimist.", Status = AnimalStatus.Adopted, CreatedAt = now.AddDays(-90) },
            new Animal { Name = "Kiku", Species = "Lind", Age = 2, Description = "Jutukas viirpapagoi, kes oskab vilistada.", Status = AnimalStatus.Available, CreatedAt = now.AddDays(-20) },
            new Animal { Name = "Mõmmi", Species = "Küülik", Age = 3, Description = "Rahulik küülik pikkade kõrvadega.", Status = AnimalStatus.Reserved, CreatedAt = now.AddDays(-50) }
        );
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
