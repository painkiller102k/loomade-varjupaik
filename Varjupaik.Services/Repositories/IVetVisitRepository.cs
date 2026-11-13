using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public interface IVetVisitRepository
{
    Task<List<VetVisit>> GetByAnimalAsync(int animalId);
    Task AddAsync(VetVisit visit);
}
