using Microsoft.Extensions.Logging;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Exceptions;
using Varjupaik.Services.Models;
using Varjupaik.Services.Notifications;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class ApplicationService
{
    private readonly IApplicationRepository _applications;
    private readonly IAnimalRepository _animals;
    private readonly IApplicationNotifier _notifier;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(IApplicationRepository applications, IAnimalRepository animals,
        IApplicationNotifier notifier, ILogger<ApplicationService> logger)
    {
        _applications = applications;
        _animals = animals;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task<ApplicationDto> CreateAsync(int animalId, int userId, ApplicationCreateDto dto)
    {
        var animal = await _animals.GetByIdAsync(animalId)
            ?? throw new NotFoundException("Looma ei leitud");

        if (animal.Status == AnimalStatus.Adopted)
        {
            throw new BadRequestException("See loom on juba adopteeritud");
        }

        if (await _applications.HasNewApplicationAsync(animalId, userId))
        {
            throw new BadRequestException("Sul on sellele loomale juba taotlus esitatud");
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
        _logger.LogInformation("Uus taotlus {ApplicationId}: kasutaja {UserId}, loom {AnimalId}", application.Id, userId, animalId);

        await NotifyStaffAsync(application.Id);
        return ApplicationDto.FromApplication(application);
    }

    // Teavitab töötajaid pärast salvestamist. Teavituse viga ei tohi taotlust ebaõnnestuda.
    private async Task NotifyStaffAsync(int applicationId)
    {
        try
        {
            var saved = await _applications.GetByIdAsync(applicationId);
            if (saved != null)
            {
                await _notifier.ApplicationReceivedAsync(ApplicationListDto.FromApplication(saved));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Töötajate teavitamine taotlusest {ApplicationId} ebaõnnestus", applicationId);
        }
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
    public async Task<ApplicationListDto> ChangeStatusAsync(int id, ApplicationStatus newStatus)
    {
        var application = await _applications.GetByIdAsync(id)
            ?? throw new NotFoundException("Taotlust ei leitud");

        if (newStatus == ApplicationStatus.New)
        {
            throw new BadRequestException("Olekuks saab valida ainult Approved või Rejected");
        }

        if (application.Status != ApplicationStatus.New)
        {
            throw new BadRequestException("See taotlus on juba menetletud");
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
        _logger.LogInformation("Taotlus {ApplicationId} muudeti olekusse {Status}, loom {AnimalId} on nüüd {AnimalStatus}",
            application.Id, newStatus, animal.Id, animal.Status);
        return ApplicationListDto.FromApplication(application);
    }
}
