using Moq;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

// ApplicationService testid mockitud repositooriumidega
public class ApplicationServiceTests
{
    private readonly Mock<IApplicationRepository> _applications = new();
    private readonly Mock<IAnimalRepository> _animals = new();
    private readonly ApplicationService _service;

    public ApplicationServiceTests()
    {
        _service = new ApplicationService(_applications.Object, _animals.Object);
    }

    private static ApplicationCreateDto Dto() => new() { Message = "Mul on suur aed ja palju aega." };

    [Fact]
    public async Task Create_AnimalNotFound_ReturnsError()
    {
        _animals.Setup(a => a.GetByIdAsync(99)).ReturnsAsync((Animal?)null);

        var (application, error) = await _service.CreateAsync(99, 1, Dto());

        Assert.Null(application);
        Assert.Equal("Looma ei leitud", error);
    }

    [Fact]
    public async Task Create_AnimalAdopted_ReturnsError()
    {
        _animals.Setup(a => a.GetByIdAsync(1)).ReturnsAsync(new Animal { Id = 1, Status = AnimalStatus.Adopted });

        var (application, error) = await _service.CreateAsync(1, 1, Dto());

        Assert.Null(application);
        Assert.NotNull(error);
        _applications.Verify(a => a.AddAsync(It.IsAny<Application>()), Times.Never);
    }

    [Fact]
    public async Task Create_AlreadyApplied_ReturnsError()
    {
        _animals.Setup(a => a.GetByIdAsync(1)).ReturnsAsync(new Animal { Id = 1, Status = AnimalStatus.Available });
        _applications.Setup(a => a.HasNewApplicationAsync(1, 5)).ReturnsAsync(true);

        var (application, error) = await _service.CreateAsync(1, 5, Dto());

        Assert.Null(application);
        Assert.Equal("Sul on sellele loomale juba taotlus esitatud", error);
    }

    [Fact]
    public async Task Create_Valid_SavesApplicationWithStatusNew()
    {
        _animals.Setup(a => a.GetByIdAsync(1)).ReturnsAsync(new Animal { Id = 1, Status = AnimalStatus.Available });
        _applications.Setup(a => a.HasNewApplicationAsync(1, 5)).ReturnsAsync(false);

        var (application, error) = await _service.CreateAsync(1, 5, Dto());

        Assert.Null(error);
        Assert.NotNull(application);
        Assert.Equal("New", application.Status);
        Assert.Equal(5, application.ApplicantUserId);
        _applications.Verify(a => a.AddAsync(It.Is<Application>(x => x.AnimalId == 1)), Times.Once);
    }
}
