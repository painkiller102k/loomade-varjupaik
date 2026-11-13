using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Varjupaik.Services;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Notifications;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

public class ApplicationServiceTests
{
    private static ApplicationCreateDto Dto() => new() { Message = "Mul on suur aed ja palju aega." };

    // Päris repositooriumid ja andmebaas mälus; teavitaja on mock, sest siin teda ei testita
    private static (ApplicationService Service, AppDbContext Db) CreateService()
    {
        var db = TestHelper.CreateDb();
        db.Users.AddRange(
            new User { Id = 1, Email = "mari@test.ee", Name = "Mari", Role = Roles.Visitor },
            new User { Id = 2, Email = "juku@test.ee", Name = "Juku", Role = Roles.Visitor });
        db.Animals.Add(new Animal { Id = 1, Name = "Muri", Species = "Koer", Age = 3, Status = AnimalStatus.Available });
        db.SaveChanges();

        var service = new ApplicationService(new ApplicationRepository(db), new AnimalRepository(db),
            Mock.Of<IApplicationNotifier>(), NullLogger<ApplicationService>.Instance);
        return (service, db);
    }

    private static AnimalStatus AnimalStatusNow(AppDbContext db) => db.Animals.AsNoTracking().First(a => a.Id == 1).Status;

    // Test 5 (mock): uus taotlus broneerib looma ja töötajaid teavitatakse ALLES PÄRAST salvestamist
    [Fact]
    public async Task Create_ReservesAnimal_AndNotifiesStaffAfterSaving()
    {
        var calls = new List<string>();
        var animal = new Animal { Id = 1, Name = "Muri", Species = "Koer", Status = AnimalStatus.Available };

        var applications = new Mock<IApplicationRepository>();
        applications.Setup(a => a.HasNewApplicationAsync(1, 5)).ReturnsAsync(false);
        applications.Setup(a => a.SaveChangesAsync()).Callback(() => calls.Add("salvestatud")).Returns(Task.CompletedTask);
        applications.Setup(a => a.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(
            new Application { AnimalId = 1, Animal = animal, ApplicantUserId = 5, ApplicantUser = new User { Name = "Mari" }, Message = "Tere" });

        var animals = new Mock<IAnimalRepository>();
        animals.Setup(a => a.GetByIdAsync(1)).ReturnsAsync(animal);

        var notifier = new Mock<IApplicationNotifier>();
        notifier.Setup(n => n.ApplicationReceivedAsync(It.IsAny<ApplicationListDto>()))
            .Callback(() => calls.Add("teavitatud")).Returns(Task.CompletedTask);

        var service = new ApplicationService(applications.Object, animals.Object, notifier.Object, NullLogger<ApplicationService>.Instance);

        await service.CreateAsync(1, 5, Dto());

        Assert.Equal(AnimalStatus.Reserved, animal.Status);
        Assert.Equal(new[] { "salvestatud", "teavitatud" }, calls);
        notifier.Verify(n => n.ApplicationReceivedAsync(It.Is<ApplicationListDto>(d => d.AnimalName == "Muri")), Times.Once);
    }

    // Test 6: kinnitamisel saab loom olekuks Adopted ja teised uued taotlused lükatakse tagasi
    [Fact]
    public async Task Approve_AnimalAdopted_AndOtherApplicationsRejected()
    {
        var (service, db) = CreateService();
        var first = await service.CreateAsync(1, 1, Dto());
        var second = await service.CreateAsync(1, 2, Dto());

        var result = await service.ChangeStatusAsync(first.Id, ApplicationStatus.Approved);

        Assert.Equal("Approved", result.Status);
        Assert.Equal(AnimalStatus.Adopted, AnimalStatusNow(db));
        Assert.Equal(ApplicationStatus.Rejected, db.Applications.AsNoTracking().First(a => a.Id == second.Id).Status);
    }

    // Test 7: viimase taotluse tagasilükkamisel on loom jälle Available
    [Fact]
    public async Task Reject_LastApplication_AnimalAvailableAgain()
    {
        var (service, db) = CreateService();
        var created = await service.CreateAsync(1, 1, Dto());
        Assert.Equal(AnimalStatus.Reserved, AnimalStatusNow(db));

        await service.ChangeStatusAsync(created.Id, ApplicationStatus.Rejected);

        Assert.Equal(AnimalStatus.Available, AnimalStatusNow(db));
    }
}
