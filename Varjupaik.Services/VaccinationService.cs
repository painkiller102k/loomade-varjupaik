using Microsoft.Extensions.Logging;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Exceptions;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class VaccinationService
{
    private readonly IVaccinationRepository _vaccinations;
    private readonly IAnimalRepository _animals;
    private readonly ILogger<VaccinationService> _logger;

    public VaccinationService(IVaccinationRepository vaccinations, IAnimalRepository animals, ILogger<VaccinationService> logger)
    {
        _vaccinations = vaccinations;
        _animals = animals;
        _logger = logger;
    }

    // Vaktsineerimised, mille tähtaeg saabub järgmise "days" päeva jooksul (ja ka juba möödunud)
    public async Task<List<UpcomingVaccinationDto>> GetUpcomingAsync(int days)
    {
        if (days < 1 || days > 365)
        {
            throw new BadRequestException("Päevade arv peab olema vahemikus 1 kuni 365");
        }

        var today = DateTime.UtcNow.Date;
        var vaccinations = await _vaccinations.GetUpcomingAsync(today.AddDays(days));

        return vaccinations.Select(v => new UpcomingVaccinationDto
        {
            Id = v.Id,
            AnimalId = v.AnimalId,
            AnimalName = v.Animal?.Name ?? "",
            AnimalSpecies = v.Animal?.Species ?? "",
            Name = v.Name,
            NextDueDate = v.NextDueDate!.Value,
            DaysLeft = (v.NextDueDate!.Value.Date - today).Days
        }).ToList();
    }

    public async Task<List<VaccinationDto>> GetByAnimalAsync(int animalId)
    {
        _ = await _animals.GetByIdAsync(animalId)
            ?? throw new NotFoundException("Looma ei leitud");

        var vaccinations = await _vaccinations.GetByAnimalAsync(animalId);
        return vaccinations.Select(VaccinationDto.FromVaccination).ToList();
    }

    public async Task<VaccinationDto> AddAsync(int animalId, VaccinationCreateDto dto)
    {
        _ = await _animals.GetByIdAsync(animalId)
            ?? throw new NotFoundException("Looma ei leitud");

        var date = dto.VaccinationDate!.Value.Date;
        if (dto.NextDueDate != null && dto.NextDueDate.Value.Date <= date)
        {
            throw new BadRequestException("Järgmine tähtaeg peab olema pärast vaktsineerimise kuupäeva");
        }

        var vaccination = new Vaccination
        {
            AnimalId = animalId,
            Name = dto.Name.Trim(),
            VaccinationDate = date,
            NextDueDate = dto.NextDueDate?.Date
        };

        await _vaccinations.AddAsync(vaccination);
        _logger.LogInformation("Vaktsineerimine lisati: loom {AnimalId}, {Name}", animalId, vaccination.Name);
        return VaccinationDto.FromVaccination(vaccination);
    }
}
