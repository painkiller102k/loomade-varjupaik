using Microsoft.EntityFrameworkCore;
using Varjupaik.Services;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

// Taotluste menetlemise testid: looma olek peab muutuma automaatselt
public class ApplicationStatusTests
{
    private readonly AppDbContext _db;
    private readonly ApplicationService _service;

    public ApplicationStatusTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        _db.Users.AddRange(
            new User { Id = 1, Email = "mari@test.ee", Name = "Mari", Role = Roles.Visitor },
            new User { Id = 2, Email = "juku@test.ee", Name = "Juku", Role = Roles.Visitor });
        _db.Animals.Add(new Animal { Id = 1, Name = "Muri", Species = "Koer", Age = 3, Status = AnimalStatus.Available });
        _db.SaveChanges();

        _service = new ApplicationService(new ApplicationRepository(_db), new AnimalRepository(_db));
    }

    private static ApplicationCreateDto Dto() => new() { Message = "Mul on suur aed ja palju aega." };

    private AnimalStatus AnimalStatusNow() => _db.Animals.AsNoTracking().First(a => a.Id == 1).Status;

    [Fact]
    public async Task NewApplication_ReservesAnimal()
    {
        await _service.CreateAsync(1, 1, Dto());

        Assert.Equal(AnimalStatus.Reserved, AnimalStatusNow());
    }

    [Fact]
    public async Task Approve_MakesAnimalAdopted()
    {
        var (created, _) = await _service.CreateAsync(1, 1, Dto());

        var (result, error, notFound) = await _service.ChangeStatusAsync(created!.Id, ApplicationStatus.Approved);

        Assert.Null(error);
        Assert.False(notFound);
        Assert.Equal("Approved", result!.Status);
        Assert.Equal(AnimalStatus.Adopted, AnimalStatusNow());
    }

    [Fact]
    public async Task Approve_RejectsOtherNewApplicationsForSameAnimal()
    {
        var (first, _) = await _service.CreateAsync(1, 1, Dto());
        var (second, _) = await _service.CreateAsync(1, 2, Dto());

        await _service.ChangeStatusAsync(first!.Id, ApplicationStatus.Approved);

        var other = _db.Applications.AsNoTracking().First(a => a.Id == second!.Id);
        Assert.Equal(ApplicationStatus.Rejected, other.Status);
    }

    [Fact]
    public async Task Reject_LastApplication_AnimalAvailableAgain()
    {
        var (created, _) = await _service.CreateAsync(1, 1, Dto());

        await _service.ChangeStatusAsync(created!.Id, ApplicationStatus.Rejected);

        Assert.Equal(AnimalStatus.Available, AnimalStatusNow());
    }

    [Fact]
    public async Task Reject_WhenAnotherNewApplicationExists_AnimalStaysReserved()
    {
        var (first, _) = await _service.CreateAsync(1, 1, Dto());
        await _service.CreateAsync(1, 2, Dto());

        await _service.ChangeStatusAsync(first!.Id, ApplicationStatus.Rejected);

        Assert.Equal(AnimalStatus.Reserved, AnimalStatusNow());
    }

    [Fact]
    public async Task ChangeStatus_AlreadyProcessed_ReturnsError()
    {
        var (created, _) = await _service.CreateAsync(1, 1, Dto());
        await _service.ChangeStatusAsync(created!.Id, ApplicationStatus.Rejected);

        var (result, error, _) = await _service.ChangeStatusAsync(created.Id, ApplicationStatus.Approved);

        Assert.Null(result);
        Assert.Equal("See taotlus on juba menetletud", error);
    }

    [Fact]
    public async Task ChangeStatus_UnknownId_ReturnsNotFound()
    {
        var (result, _, notFound) = await _service.ChangeStatusAsync(999, ApplicationStatus.Approved);

        Assert.Null(result);
        Assert.True(notFound);
    }

    [Fact]
    public async Task ChangeStatus_BackToNew_IsNotAllowed()
    {
        var (created, _) = await _service.CreateAsync(1, 1, Dto());

        var (_, error, _) = await _service.ChangeStatusAsync(created!.Id, ApplicationStatus.New);

        Assert.NotNull(error);
        Assert.Equal(AnimalStatus.Reserved, AnimalStatusNow());
    }

    [Fact]
    public async Task GetAll_FiltersByStatus_AndMyShowsOnlyOwn()
    {
        var (first, _) = await _service.CreateAsync(1, 1, Dto());
        await _service.CreateAsync(1, 2, Dto());
        await _service.ChangeStatusAsync(first!.Id, ApplicationStatus.Approved);

        var approved = await _service.GetAllAsync(ApplicationStatus.Approved);
        var all = await _service.GetAllAsync(null);
        var mine = await _service.GetMyAsync(2);

        Assert.Single(approved);
        Assert.Equal("Mari", approved[0].ApplicantName);
        Assert.Equal("Muri", approved[0].AnimalName);
        Assert.Equal(2, all.Count);
        Assert.Single(mine);
        Assert.Equal("Juku", mine[0].ApplicantName);
    }
}
