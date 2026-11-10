using Varjupaik.Services.Data;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public class AnimalRepository : IAnimalRepository
{
    private readonly AppDbContext _db;

    public AnimalRepository(AppDbContext db)
    {
        _db = db;
    }

    public IQueryable<Animal> Query()
    {
        return _db.Animals.AsQueryable();
    }

    public async Task<Animal?> GetByIdAsync(int id)
    {
        return await _db.Animals.FindAsync(id);
    }

    public async Task AddAsync(Animal animal)
    {
        _db.Animals.Add(animal);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Animal animal)
    {
        _db.Animals.Update(animal);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Animal animal)
    {
        _db.Animals.Remove(animal);
        await _db.SaveChangesAsync();
    }
}
