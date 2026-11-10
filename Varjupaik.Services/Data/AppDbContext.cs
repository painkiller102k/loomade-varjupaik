using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Application> Applications => Set<Application>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Olek salvestatakse tekstina, et andmebaasis oleks loetav
        modelBuilder.Entity<Animal>().Property(a => a.Status).HasConversion<string>();
        modelBuilder.Entity<Application>().Property(a => a.Status).HasConversion<string>();
    }
}
