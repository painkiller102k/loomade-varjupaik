using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public interface IApplicationRepository
{
    Task<bool> HasNewApplicationAsync(int animalId, int userId);
    Task<List<Application>> GetAllAsync(ApplicationStatus? status);
    Task<List<Application>> GetByUserAsync(int userId);
    Task<Application?> GetByIdAsync(int id);
    Task<List<Application>> GetNewByAnimalAsync(int animalId);
    void Add(Application application);

    // Salvestab kõik muudatused korraga (taotlus + looma olek) ühes tehingus
    Task SaveChangesAsync();
}
