using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public class VetVisitRepository : IVetVisitRepository
{
    private readonly AppDbContext _db;

    public VetVisitRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<VetVisit>> GetByAnimalAsync(int animalId)
    {
        return await _db.VetVisits
            .Where(v => v.AnimalId == animalId)
            .OrderByDescending(v => v.VisitDate)
            .ThenByDescending(v => v.Id)
            .ToListAsync();
    }

    public async Task AddAsync(VetVisit visit)
    {
        _db.VetVisits.Add(visit);
        await _db.SaveChangesAsync();
    }
}
