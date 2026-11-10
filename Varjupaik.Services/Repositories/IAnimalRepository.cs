using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public interface IAnimalRepository
{
    IQueryable<Animal> Query();
    Task<Animal?> GetByIdAsync(int id);
    Task AddAsync(Animal animal);
    Task UpdateAsync(Animal animal);
    Task DeleteAsync(Animal animal);
}
