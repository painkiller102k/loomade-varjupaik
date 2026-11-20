using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _db;

    public NotificationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Notification notification)
    {
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetLatestAsync(int count)
    {
        return await _db.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ThenByDescending(n => n.Id)
            .Take(count)
            .ToListAsync();
    }
}
