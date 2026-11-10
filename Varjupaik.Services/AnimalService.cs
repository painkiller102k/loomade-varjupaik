using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class AnimalService
{
    private readonly IAnimalRepository _animals;

    public AnimalService(IAnimalRepository animals)
    {
        _animals = animals;
    }

    // Avalik nimekiri, filtrid on valikulised
    public async Task<List<AnimalDto>> GetAllAsync(string? species, AnimalStatus? status)
    {
        var query = _animals.Query();

        if (!string.IsNullOrWhiteSpace(species))
        {
            var s = species.Trim().ToLower();
            query = query.Where(a => a.Species.ToLower() == s);
        }

        if (status != null)
        {
            query = query.Where(a => a.Status == status);
        }

        var animals = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        return animals.Select(AnimalDto.FromAnimal).ToList();
    }

    public async Task<AnimalDto?> GetByIdAsync(int id)
    {
        var animal = await _animals.GetByIdAsync(id);
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

        await _animals.AddAsync(animal);
        return AnimalDto.FromAnimal(animal);
    }

    public async Task<AnimalDto?> UpdateAsync(int id, AnimalSaveDto dto, string? photoPath)
    {
        var animal = await _animals.GetByIdAsync(id);
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

        await _animals.UpdateAsync(animal);
        return AnimalDto.FromAnimal(animal);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var animal = await _animals.GetByIdAsync(id);
        if (animal == null)
        {
            return false;
        }

        await _animals.DeleteAsync(animal);
        return true;
    }
}
