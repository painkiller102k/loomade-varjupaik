using System.ComponentModel.DataAnnotations;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Dtos;

public class ApplicationCreateDto
{
    /// <summary>Miks soovid looma adopteerida</summary>
    /// <example>Mul on suur aed ja palju aega koeraga jalutamiseks.</example>
    [Required, MinLength(10), MaxLength(1000)]
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

// Taotlus koos looma ja taotleja andmetega (töötajate ja külastaja nimekirjade jaoks)
public class ApplicationListDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string AnimalName { get; set; } = "";
    public string AnimalSpecies { get; set; } = "";
    public string AnimalStatus { get; set; } = "";
    public int ApplicantUserId { get; set; }
    public string ApplicantName { get; set; } = "";
    public string ApplicantEmail { get; set; } = "";
    public string Message { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public static ApplicationListDto FromApplication(Application a)
    {
        return new ApplicationListDto
        {
            Id = a.Id,
            AnimalId = a.AnimalId,
            AnimalName = a.Animal?.Name ?? "",
            AnimalSpecies = a.Animal?.Species ?? "",
            AnimalStatus = a.Animal?.Status.ToString() ?? "",
            ApplicantUserId = a.ApplicantUserId,
            ApplicantName = a.ApplicantUser?.Name ?? "",
            ApplicantEmail = a.ApplicantUser?.Email ?? "",
            Message = a.Message,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt
        };
    }
}

public class ApplicationStatusDto
{
    /// <summary>Uus olek: Approved (kinnitatud) või Rejected (tagasi lükatud)</summary>
    /// <example>Approved</example>
    [Required]
    public ApplicationStatus Status { get; set; }
}
