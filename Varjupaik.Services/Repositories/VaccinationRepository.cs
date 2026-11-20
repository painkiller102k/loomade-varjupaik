using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public class VaccinationRepository : IVaccinationRepository
{
    private readonly AppDbContext _db;

    public VaccinationRepository(AppDbContext db)
    {
        _db = db;
    }

    // Vaktsineerimised, mille tähtaeg on kuni "until" (ka üle tähtaja), adopteeritud loomi ei arvesta
    public async Task<List<Vaccination>> GetUpcomingAsync(DateTime until)
    {
        return await _db.Vaccinations
            .Include(v => v.Animal)
            .Where(v => v.NextDueDate != null && v.NextDueDate <= until && v.Animal!.Status != AnimalStatus.Adopted)
            .OrderBy(v => v.NextDueDate)
            .ToListAsync();
    }

    public async Task<List<Vaccination>> GetByAnimalAsync(int animalId)
    {
        return await _db.Vaccinations
            .Where(v => v.AnimalId == animalId)
            .OrderByDescending(v => v.VaccinationDate)
            .ThenByDescending(v => v.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Vaccination vaccination)
    {
        _db.Vaccinations.Add(vaccination);
        await _db.SaveChangesAsync();
    }
}
