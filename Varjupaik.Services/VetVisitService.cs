using Microsoft.Extensions.Logging;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Exceptions;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class VetVisitService
{
    private readonly IVetVisitRepository _visits;
    private readonly IAnimalRepository _animals;
    private readonly ILogger<VetVisitService> _logger;

    public VetVisitService(IVetVisitRepository visits, IAnimalRepository animals, ILogger<VetVisitService> logger)
    {
        _visits = visits;
        _animals = animals;
        _logger = logger;
    }

    // Looma visiitide ajalugu (uuemad enne)
    public async Task<List<VetVisitDto>> GetHistoryAsync(int animalId)
    {
        _ = await _animals.GetByIdAsync(animalId)
            ?? throw new NotFoundException("Looma ei leitud");

        var visits = await _visits.GetByAnimalAsync(animalId);
        return visits.Select(VetVisitDto.FromVisit).ToList();
    }

    // Lisab visiidi
    public async Task<VetVisitDto> AddAsync(int animalId, VetVisitCreateDto dto)
    {
        _ = await _animals.GetByIdAsync(animalId)
            ?? throw new NotFoundException("Looma ei leitud");

        var visit = new VetVisit
        {
            AnimalId = animalId,
            VisitDate = dto.VisitDate!.Value.Date,
            Reason = dto.Reason.Trim(),
            Notes = dto.Notes.Trim(),
            Vet = dto.Vet.Trim()
        };

        await _visits.AddAsync(visit);
        _logger.LogInformation("Veterinaarivisiit lisati: loom {AnimalId}, põhjus {Reason}", animalId, visit.Reason);
        return VetVisitDto.FromVisit(visit);
    }
}
