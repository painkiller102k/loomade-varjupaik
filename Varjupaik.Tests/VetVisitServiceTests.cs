using Microsoft.EntityFrameworkCore;
using Moq;
using Varjupaik.Services;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

public class VetVisitServiceTests
{
    private static VetVisitCreateDto Dto(string reason = "Vaktsiin", string date = "2026-11-10") => new()
    {
        VisitDate = DateTime.Parse(date),
        Reason = reason,
        Notes = "Kõik hästi.",
        Vet = "Dr. Tamm"
    };

    [Fact]
    public async Task Add_UnknownAnimal_ReturnsNull_AndSavesNothing()
    {
        var visits = new Mock<IVetVisitRepository>();
        var animals = new Mock<IAnimalRepository>();
        animals.Setup(a => a.GetByIdAsync(99)).ReturnsAsync((Animal?)null);
        var service = new VetVisitService(visits.Object, animals.Object);

        var result = await service.AddAsync(99, Dto());

        Assert.Null(result);
        visits.Verify(v => v.AddAsync(It.IsAny<VetVisit>()), Times.Never);
    }

    [Fact]
    public async Task Add_TrimsTextAndSavesVisit()
    {
        var visits = new Mock<IVetVisitRepository>();
        var animals = new Mock<IAnimalRepository>();
        animals.Setup(a => a.GetByIdAsync(1)).ReturnsAsync(new Animal { Id = 1 });
        var service = new VetVisitService(visits.Object, animals.Object);

        var result = await service.AddAsync(1, Dto("  Vaktsiin  "));

        Assert.NotNull(result);
        Assert.Equal("Vaktsiin", result.Reason);
        visits.Verify(v => v.AddAsync(It.Is<VetVisit>(x => x.AnimalId == 1 && x.Vet == "Dr. Tamm")), Times.Once);
    }

    private static (VetVisitService Service, AppDbContext Db) CreateWithDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(options);
        db.Animals.AddRange(new Animal { Id = 1, Name = "Muri" }, new Animal { Id = 2, Name = "Miisu" });
        db.SaveChanges();
        return (new VetVisitService(new VetVisitRepository(db), new AnimalRepository(db)), db);
    }

    [Fact]
    public async Task History_IsNewestFirst_AndOnlyForThatAnimal()
    {
        var (service, _) = CreateWithDb();
        await service.AddAsync(1, Dto("Vana", "2026-01-05"));
        await service.AddAsync(1, Dto("Uus", "2026-10-20"));
        await service.AddAsync(2, Dto("Teise looma", "2026-11-01"));

        var history = await service.GetHistoryAsync(1);

        Assert.NotNull(history);
        Assert.Equal(new[] { "Uus", "Vana" }, history.Select(v => v.Reason));
    }

    [Fact]
    public async Task History_UnknownAnimal_ReturnsNull()
    {
        var (service, _) = CreateWithDb();

        Assert.Null(await service.GetHistoryAsync(999));
    }

    [Fact]
    public async Task DeletingAnimal_AlsoDeletesItsVisitsAndVaccinations()
    {
        var (service, db) = CreateWithDb();
        await service.AddAsync(1, Dto());
        db.Vaccinations.Add(new Vaccination { AnimalId = 1, Name = "Marutaudi", VaccinationDate = DateTime.UtcNow });
        db.SaveChanges();

        db.Animals.Remove(db.Animals.First(a => a.Id == 1));
        db.SaveChanges();

        Assert.Empty(db.VetVisits);
        Assert.Empty(db.Vaccinations);
    }
}
