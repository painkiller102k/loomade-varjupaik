using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class StatisticsService
{
    private readonly IAnimalRepository _animals;

    public StatisticsService(IAnimalRepository animals)
    {
        _animals = animals;
    }

    // Keskmine aeg varjupaigas liigi kaupa.
    // Adopteeritud loomal: lisamisest adopteerimiseni. Teistel: lisamisest tänaseni.
    public async Task<List<AverageStayDto>> GetAverageStayAsync()
    {
        var now = DateTime.UtcNow;

        var animals = await _animals.Query()
            .Select(a => new { a.Species, a.CreatedAt, a.AdoptedAt })
            .ToListAsync();

        return animals
            .GroupBy(a => a.Species.Trim().ToLower())
            .Select(g => new AverageStayDto
            {
                Species = g.First().Species,
                AnimalCount = g.Count(),
                AverageDays = Math.Round(g.Average(a => Math.Max(0, ((a.AdoptedAt ?? now) - a.CreatedAt).TotalDays)), 1)
            })
            .OrderBy(s => s.Species)
            .ToList();
    }
}
