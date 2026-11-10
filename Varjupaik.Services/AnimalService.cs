using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;

namespace Varjupaik.Services;

public class AnimalService
{
    private readonly AppDbContext _db;

    public AnimalService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AnimalDto?> GetByIdAsync(int id)
    {
        var animal = await _db.Animals.FindAsync(id);
        return animal == null ? null : AnimalDto.FromAnimal(animal);
    }

    public async Task<AnimalDto> CreateAsync(AnimalSaveDto dto, string? photoPath)
    {
        var animal = new Animal
        {
            Name = dto.Name,
            Species = dto.Species,
            Age = dto.Age,
            Description = dto.Description,
            Status = dto.Status,
            PhotoPath = photoPath
        };

        _db.Animals.Add(animal);
        await _db.SaveChangesAsync();
        return AnimalDto.FromAnimal(animal);
    }

    public async Task<AnimalDto?> UpdateAsync(int id, AnimalSaveDto dto, string? photoPath)
    {
        var animal = await _db.Animals.FindAsync(id);
        if (animal == null)
        {
            return null;
        }

        animal.Name = dto.Name;
        animal.Species = dto.Species;
        animal.Age = dto.Age;
        animal.Description = dto.Description;
        animal.Status = dto.Status;

        // Pilti muudame ainult siis, kui laaditi uus pilt
        if (photoPath != null)
        {
            animal.PhotoPath = photoPath;
        }

        await _db.SaveChangesAsync();
        return AnimalDto.FromAnimal(animal);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var animal = await _db.Animals.FindAsync(id);
        if (animal == null)
        {
            return false;
        }

        _db.Animals.Remove(animal);
        await _db.SaveChangesAsync();
        return true;
    }
}
