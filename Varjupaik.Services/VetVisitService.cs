using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class VetVisitService
{
    private readonly IVetVisitRepository _visits;
    private readonly IAnimalRepository _animals;

    public VetVisitService(IVetVisitRepository visits, IAnimalRepository animals)
    {
        _visits = visits;
        _animals = animals;
    }

    // Looma visiitide ajalugu (uuemad enne). Tagastab null, kui looma ei leitud.
    public async Task<List<VetVisitDto>?> GetHistoryAsync(int animalId)
    {
        var animal = await _animals.GetByIdAsync(animalId);
        if (animal == null)
        {
            return null;
        }

        var visits = await _visits.GetByAnimalAsync(animalId);
        return visits.Select(VetVisitDto.FromVisit).ToList();
    }

    // Lisab visiidi. Tagastab null, kui looma ei leitud.
    public async Task<VetVisitDto?> AddAsync(int animalId, VetVisitCreateDto dto)
    {
        var animal = await _animals.GetByIdAsync(animalId);
        if (animal == null)
        {
            return null;
        }

        var visit = new VetVisit
        {
            AnimalId = animalId,
            VisitDate = dto.VisitDate!.Value.Date,
            Reason = dto.Reason.Trim(),
            Notes = dto.Notes.Trim(),
            Vet = dto.Vet.Trim()
        };

        await _visits.AddAsync(visit);
        return VetVisitDto.FromVisit(visit);
    }
}
