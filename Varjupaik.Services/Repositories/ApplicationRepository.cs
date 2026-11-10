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

    public async Task AddAsync(Application application)
    {
        _db.Applications.Add(application);
        await _db.SaveChangesAsync();
    }
}
