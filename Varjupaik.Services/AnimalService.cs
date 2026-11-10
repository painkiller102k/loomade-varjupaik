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

    // Avalik nimekiri: filtrid, otsing, sorteerimine ja leheküljed
    public async Task<PagedResult<AnimalDto>> GetAllAsync(AnimalQuery q)
    {
        var query = _animals.Query();

        if (!string.IsNullOrWhiteSpace(q.Species))
        {
            var species = q.Species.Trim().ToLower();
            query = query.Where(a => a.Species.ToLower() == species);
        }

        if (q.Status != null)
        {
            query = query.Where(a => a.Status == q.Status);
        }

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var search = q.Search.Trim().ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(search) || a.Description.ToLower().Contains(search));
        }

        query = q.SortBy switch
        {
            "name" => query.OrderBy(a => a.Name),
            "age" => query.OrderBy(a => a.Age),
            "oldest" => query.OrderBy(a => a.CreatedAt),
            _ => query.OrderByDescending(a => a.CreatedAt)
        };

        var page = q.Page < 1 ? 1 : q.Page;
        var pageSize = q.PageSize < 1 ? 9 : Math.Min(q.PageSize, 50);

        var totalCount = await query.CountAsync();
        var animals = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<AnimalDto>
        {
            Items = animals.Select(AnimalDto.FromAnimal).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
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
