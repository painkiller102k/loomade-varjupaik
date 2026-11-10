using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public interface IApplicationRepository
{
    Task<bool> HasNewApplicationAsync(int animalId, int userId);
    Task AddAsync(Application application);
}
