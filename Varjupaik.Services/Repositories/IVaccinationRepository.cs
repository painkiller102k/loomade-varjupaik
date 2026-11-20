using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public interface IVaccinationRepository
{
    Task<List<Vaccination>> GetUpcomingAsync(DateTime until);
    Task<List<Vaccination>> GetByAnimalAsync(int animalId);
    Task AddAsync(Vaccination vaccination);
}
