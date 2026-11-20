using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<List<Notification>> GetLatestAsync(int count);
}
