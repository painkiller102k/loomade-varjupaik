using Microsoft.EntityFrameworkCore;
using Varjupaik.Services;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

// AnimalService testid päris repositooriumiga ja mälus oleva andmebaasiga
public class AnimalServiceTests
{
    private static AnimalService CreateService(out AppDbContext db)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        db = new AppDbContext(options);

        db.Animals.AddRange(
            new Animal { Name = "Muri", Species = "Koer", Age = 3, Description = "Sõbralik koer", Status = AnimalStatus.Available, CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new Animal { Name = "Miisu", Species = "Kass", Age = 2, Description = "Rahulik kass", Status = AnimalStatus.Available, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new Animal { Name = "Reks", Species = "Koer", Age = 6, Description = "Tark koer", Status = AnimalStatus.Reserved, CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new Animal { Name = "Tom", Species = "Kass", Age = 9, Description = "Vana sõbralik kass", Status = AnimalStatus.Adopted, CreatedAt = DateTime.UtcNow.AddDays(-1) }
        );
        db.SaveChanges();

        return new AnimalService(new AnimalRepository(db));
    }

    [Fact]
    public async Task GetAll_FilterBySpeciesAndStatus()
    {
        var service = CreateService(out _);

        var result = await service.GetAllAsync(new AnimalQuery { Species = "koer", Status = AnimalStatus.Available });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Muri", result.Items[0].Name);
    }

    [Fact]
    public async Task GetAll_SearchFindsNameAndDescription()
    {
        var service = CreateService(out _);

        var result = await service.GetAllAsync(new AnimalQuery { Search = "sõbralik" });

        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public async Task GetAll_SortByAge()
    {
        var service = CreateService(out _);

        var result = await service.GetAllAsync(new AnimalQuery { SortBy = "age" });

        Assert.Equal(new[] { "Miisu", "Muri", "Reks", "Tom" }, result.Items.Select(a => a.Name));
    }

    [Fact]
    public async Task GetAll_Pagination()
    {
        var service = CreateService(out _);

        var result = await service.GetAllAsync(new AnimalQuery { SortBy = "name", Page = 2, PageSize = 3 });

        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Single(result.Items);
        Assert.Equal("Tom", result.Items[0].Name);
    }

    [Fact]
    public async Task Update_WithoutNewPhoto_KeepsOldPhoto()
    {
        var service = CreateService(out var db);
        var muri = db.Animals.First(a => a.Name == "Muri");
        muri.PhotoPath = "/uploads/muri.jpg";
        db.SaveChanges();

        var updated = await service.UpdateAsync(muri.Id,
            new AnimalSaveDto { Name = "Muri", Species = "Koer", Age = 4, Description = "Uus", Status = AnimalStatus.Reserved }, null);

        Assert.NotNull(updated);
        Assert.Equal("/uploads/muri.jpg", updated.PhotoUrl);
        Assert.Equal("Reserved", updated.Status);
        Assert.Equal(4, updated.Age);
    }

    [Fact]
    public async Task Delete_NotExisting_ReturnsFalse()
    {
        var service = CreateService(out var db);

        var deleted = await service.DeleteAsync(12345);

        Assert.False(deleted);
        Assert.Equal(4, db.Animals.Count());
    }
}
