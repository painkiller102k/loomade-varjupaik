using Microsoft.Extensions.Logging;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Notifications;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Services;

public class NotificationService
{
    private readonly INotificationRepository _notifications;
    private readonly IApplicationNotifier _notifier;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository notifications, IApplicationNotifier notifier, ILogger<NotificationService> logger)
    {
        _notifications = notifications;
        _notifier = notifier;
        _logger = logger;
    }

    // Salvestab teavituse ja saadab selle pärast salvestamist töötajatele (WebSocket)
    public async Task<NotificationDto> CreateAsync(NotificationCreateDto dto)
    {
        var notification = new Notification
        {
            Title = dto.Title.Trim(),
            Message = dto.Message.Trim()
        };

        await _notifications.AddAsync(notification);
        _logger.LogInformation("Teavitus {NotificationId} salvestati: {Title}", notification.Id, notification.Title);

        var result = NotificationDto.FromNotification(notification);
        try
        {
            await _notifier.NotificationCreatedAsync(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Teavituse {NotificationId} saatmine töötajatele ebaõnnestus", notification.Id);
        }

        return result;
    }

    public async Task<List<NotificationDto>> GetLatestAsync(int count = 20)
    {
        var notifications = await _notifications.GetLatestAsync(count);
        return notifications.Select(NotificationDto.FromNotification).ToList();
    }
}
