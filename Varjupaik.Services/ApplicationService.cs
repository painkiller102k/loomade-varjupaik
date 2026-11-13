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
        _applications.Add(application);

        // Uus taotlus broneerib looma. Taotlus ja looma olek salvestatakse ühe SaveChanges kutsega,
        // seega ühes tehingus.
        if (animal.Status == AnimalStatus.Available)
        {
            animal.Status = AnimalStatus.Reserved;
        }

        await _applications.SaveChangesAsync();
        return (ApplicationDto.FromApplication(application), null);
    }

    // Kõik taotlused töötajatele, valikuline filter oleku järgi
    public async Task<List<ApplicationListDto>> GetAllAsync(ApplicationStatus? status)
    {
        var applications = await _applications.GetAllAsync(status);
        return applications.Select(ApplicationListDto.FromApplication).ToList();
    }

    // Külastaja näeb ainult enda taotlusi
    public async Task<List<ApplicationListDto>> GetMyAsync(int userId)
    {
        var applications = await _applications.GetByUserAsync(userId);
        return applications.Select(ApplicationListDto.FromApplication).ToList();
    }

    // Taotluse menetlemine. Looma olek muutub samas tehingus:
    //  - Approved: loom saab olekuks Adopted ja kõik teised uued taotlused sellele loomale lükatakse tagasi
    //  - Rejected: kui uusi taotlusi enam pole, muutub broneeritud loom jälle Available
    public async Task<(ApplicationListDto? Application, string? Error, bool NotFound)> ChangeStatusAsync(int id, ApplicationStatus newStatus)
    {
        var application = await _applications.GetByIdAsync(id);
        if (application == null)
        {
            return (null, "Taotlust ei leitud", true);
        }

        if (newStatus == ApplicationStatus.New)
        {
            return (null, "Olekuks saab valida ainult Approved või Rejected", false);
        }

        if (application.Status != ApplicationStatus.New)
        {
            return (null, "See taotlus on juba menetletud", false);
        }

        var animal = application.Animal!;
        application.Status = newStatus;

        var otherNew = (await _applications.GetNewByAnimalAsync(animal.Id))
            .Where(a => a.Id != application.Id)
            .ToList();

        if (newStatus == ApplicationStatus.Approved)
        {
            animal.Status = AnimalStatus.Adopted;
            foreach (var other in otherNew)
            {
                other.Status = ApplicationStatus.Rejected;
            }
        }
        else if (otherNew.Count == 0 && animal.Status == AnimalStatus.Reserved)
        {
            animal.Status = AnimalStatus.Available;
        }

        // Taotluse, looma ja teiste taotluste muudatused salvestatakse korraga
        await _applications.SaveChangesAsync();
        return (ApplicationListDto.FromApplication(application), null, false);
    }
}
