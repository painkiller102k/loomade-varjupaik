using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;

namespace Varjupaik.Services;

public class ApplicationService
{
    private readonly AppDbContext _db;

    public ApplicationService(AppDbContext db)
    {
        _db = db;
    }

    // Tagastab (taotlus, veateade). Kui veateade pole null, siis taotlust ei loodud.
    public async Task<(ApplicationDto? Application, string? Error)> CreateAsync(int animalId, int userId, ApplicationCreateDto dto)
    {
        var animal = await _db.Animals.FindAsync(animalId);
        if (animal == null)
        {
            return (null, "Looma ei leitud");
        }

        if (animal.Status == AnimalStatus.Adopted)
        {
            return (null, "See loom on juba adopteeritud");
        }

        var alreadyApplied = await _db.Applications.AnyAsync(a =>
            a.AnimalId == animalId && a.ApplicantUserId == userId && a.Status == ApplicationStatus.New);
        if (alreadyApplied)
        {
            return (null, "Sul on sellele loomale juba taotlus esitatud");
        }

        var application = new Application
        {
            AnimalId = animalId,
            ApplicantUserId = userId,
            Message = dto.Message
        };

        _db.Applications.Add(application);
        await _db.SaveChangesAsync();
        return (ApplicationDto.FromApplication(application), null);
    }
}
