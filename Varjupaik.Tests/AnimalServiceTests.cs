using Microsoft.Extensions.Logging.Abstractions;
using Varjupaik.Services;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

public class AnimalServiceTests
{
    private static AnimalService CreateService(AppDbContext db)
    {
        db.Animals.AddRange(
            new Animal { Name = "Muri", Species = "Koer", Age = 3, Description = "Sõbralik koer", Status = AnimalStatus.Available },
            new Animal { Name = "Miisu", Species = "Kass", Age = 2, Description = "Rahulik kass", Status = AnimalStatus.Available },
            new Animal { Name = "Reks", Species = "Koer", Age = 6, Description = "Tark koer", Status = AnimalStatus.Reserved },
            new Animal { Name = "Tom", Species = "Kass", Age = 9, Description = "Vana kass", Status = AnimalStatus.Adopted });
        db.SaveChanges();
        return new AnimalService(new AnimalRepository(db), NullLogger<AnimalService>.Instance);
    }

    // Test 3: avalik nimekiri filtreerib liigi ja oleku järgi
    [Fact]
    public async Task GetAll_FilterBySpeciesAndStatus()
    {
        var service = CreateService(TestHelper.CreateDb());

        var result = await service.GetAllAsync(new AnimalQuery { Species = "koer", Status = AnimalStatus.Available });

        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Muri", result.Items[0].Name);
    }

    // Test 4: leheküljed ja sorteerimine
    [Fact]
    public async Task GetAll_SortedAndPaged()
    {
        var service = CreateService(TestHelper.CreateDb());

        var result = await service.GetAllAsync(new AnimalQuery { SortBy = "name", Page = 2, PageSize = 3 });

        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal("Tom", Assert.Single(result.Items).Name);
    }
}
