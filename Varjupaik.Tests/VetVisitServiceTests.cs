using Microsoft.Extensions.Logging.Abstractions;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Exceptions;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

public class VetVisitServiceTests
{
    // Test 8: ajalugu on uuemad enne, ainult selle looma visiidid; tundmatu loom annab 404
    [Fact]
    public async Task History_NewestFirst_OnlyForThatAnimal_AndUnknownAnimalIsNotFound()
    {
        var db = TestHelper.CreateDb();
        db.Animals.AddRange(new Animal { Id = 1, Name = "Muri" }, new Animal { Id = 2, Name = "Miisu" });
        db.SaveChanges();
        var service = new VetVisitService(new VetVisitRepository(db), new AnimalRepository(db), NullLogger<VetVisitService>.Instance);

        VetVisitCreateDto Visit(string reason, string date) =>
            new() { VisitDate = DateTime.Parse(date), Reason = reason, Notes = "", Vet = "Dr. Tamm" };

        await service.AddAsync(1, Visit("Vana", "2026-01-05"));
        await service.AddAsync(1, Visit("Uus", "2026-10-20"));
        await service.AddAsync(2, Visit("Teise looma", "2026-11-01"));

        var history = await service.GetHistoryAsync(1);

        Assert.Equal(new[] { "Uus", "Vana" }, history.Select(v => v.Reason));
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetHistoryAsync(999));
    }
}
