using System.ComponentModel.DataAnnotations;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Dtos;

public class ApplicationCreateDto
{
    [Required, MaxLength(1000)]
    public string Message { get; set; } = "";
}

public class ApplicationDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public int ApplicantUserId { get; set; }
    public string Message { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public static ApplicationDto FromApplication(Application a)
    {
        return new ApplicationDto
        {
            Id = a.Id,
            AnimalId = a.AnimalId,
            ApplicantUserId = a.ApplicantUserId,
            Message = a.Message,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt
        };
    }
}
