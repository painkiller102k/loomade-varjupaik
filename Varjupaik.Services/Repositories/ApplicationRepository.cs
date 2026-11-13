using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly AppDbContext _db;

    public ApplicationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasNewApplicationAsync(int animalId, int userId)
    {
        return await _db.Applications.AnyAsync(a =>
            a.AnimalId == animalId && a.ApplicantUserId == userId && a.Status == ApplicationStatus.New);
    }

    public async Task<List<Application>> GetAllAsync(ApplicationStatus? status)
    {
        var query = _db.Applications
            .Include(a => a.Animal)
            .Include(a => a.ApplicantUser)
            .AsQueryable();

        if (status != null)
        {
            query = query.Where(a => a.Status == status);
        }

        return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task<List<Application>> GetByUserAsync(int userId)
    {
        return await _db.Applications
            .Include(a => a.Animal)
            .Include(a => a.ApplicantUser)
            .Where(a => a.ApplicantUserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Application?> GetByIdAsync(int id)
    {
        return await _db.Applications
            .Include(a => a.Animal)
            .Include(a => a.ApplicantUser)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Application>> GetNewByAnimalAsync(int animalId)
    {
        return await _db.Applications
            .Where(a => a.AnimalId == animalId && a.Status == ApplicationStatus.New)
            .ToListAsync();
    }

    public void Add(Application application)
    {
        _db.Applications.Add(application);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}
