using System.ComponentModel.DataAnnotations;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Dtos;

public class VetVisitCreateDto
{
    /// <summary>Visiidi kuupäev</summary>
    /// <example>2026-11-10</example>
    [Required]
    public DateTime? VisitDate { get; set; }

    /// <summary>Visiidi põhjus</summary>
    /// <example>Marutaudi vaktsiin</example>
    [Required, MaxLength(200)]
    public string Reason { get; set; } = "";

    /// <summary>Arsti märkused</summary>
    /// <example>Reaktsioone ei olnud.</example>
    [MaxLength(1000)]
    public string Notes { get; set; } = "";

    /// <summary>Veterinaari nimi</summary>
    /// <example>Dr. Tamm</example>
    [Required, MaxLength(100)]
    public string Vet { get; set; } = "";
}

public class VetVisitDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public DateTime VisitDate { get; set; }
    public string Reason { get; set; } = "";
    public string Notes { get; set; } = "";
    public string Vet { get; set; } = "";

    public static VetVisitDto FromVisit(VetVisit v)
    {
        return new VetVisitDto
        {
            Id = v.Id,
            AnimalId = v.AnimalId,
            VisitDate = v.VisitDate,
            Reason = v.Reason,
            Notes = v.Notes,
            Vet = v.Vet
        };
    }
}
