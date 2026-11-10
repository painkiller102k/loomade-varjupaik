using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class ApplicationService
{
    private readonly IApplicationRepository _applications;
    private readonly IAnimalRepository _animals;

    public ApplicationService(IApplicationRepository applications, IAnimalRepository animals)
    {
        _applications = applications;
        _animals = animals;
    }

    // Tagastab (taotlus, veateade). Kui veateade pole null, siis taotlust ei loodud.
    public async Task<(ApplicationDto? Application, string? Error)> CreateAsync(int animalId, int userId, ApplicationCreateDto dto)
    {
        var animal = await _animals.GetByIdAsync(animalId);
        if (animal == null)
        {
            return (null, "Looma ei leitud");
        }

        if (animal.Status == AnimalStatus.Adopted)
        {
            return (null, "See loom on juba adopteeritud");
        }

        var alreadyApplied = await _applications.HasNewApplicationAsync(animalId, userId);
        if (alreadyApplied)
        {
            return (null, "Sul on sellele loomale juba taotlus esitatud");
        }

        var application = new Application
        {
            AnimalId = animalId,
            ApplicantUserId = userId,
            Message = dto.Message
        };

        await _applications.AddAsync(application);
        return (ApplicationDto.FromApplication(application), null);
    }
}
