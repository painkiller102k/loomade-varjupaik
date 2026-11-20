using System.ComponentModel.DataAnnotations;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Dtos;

public class NotificationCreateDto
{
    /// <summary>Teavituse pealkiri</summary>
    /// <example>Lähenev vaktsineerimine</example>
    [Required, MaxLength(100)]
    public string Title { get; set; } = "";

    /// <summary>Teavituse tekst</summary>
    /// <example>Muri: Leetospiroos tuleb teha 5 päeva pärast.</example>
    [Required, MaxLength(500)]
    public string Message { get; set; } = "";
}

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public static NotificationDto FromNotification(Notification n)
    {
        return new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            CreatedAt = n.CreatedAt
        };
    }
}
