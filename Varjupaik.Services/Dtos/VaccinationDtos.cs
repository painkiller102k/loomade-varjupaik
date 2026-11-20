using System.ComponentModel.DataAnnotations;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Dtos;

public class VaccinationCreateDto
{
    /// <summary>Vaktsiini nimi</summary>
    /// <example>Marutaudi</example>
    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    /// <summary>Millal vaktsineeriti</summary>
    /// <example>2026-11-10</example>
    [Required]
    public DateTime? VaccinationDate { get; set; }

    /// <summary>Millal tuleb järgmine kord vaktsineerida (valikuline)</summary>
    /// <example>2027-11-10</example>
    public DateTime? NextDueDate { get; set; }
}

public class VaccinationDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string Name { get; set; } = "";
    public DateTime VaccinationDate { get; set; }
    public DateTime? NextDueDate { get; set; }

    public static VaccinationDto FromVaccination(Vaccination v)
    {
        return new VaccinationDto
        {
            Id = v.Id,
            AnimalId = v.AnimalId,
            Name = v.Name,
            VaccinationDate = v.VaccinationDate,
            NextDueDate = v.NextDueDate
        };
    }
}

// Lähenev vaktsineerimine koos looma andmetega
public class UpcomingVaccinationDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public string AnimalName { get; set; } = "";
    public string AnimalSpecies { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime NextDueDate { get; set; }

    /// <summary>Päevi tähtajani, negatiivne tähendab, et tähtaeg on möödas</summary>
    public int DaysLeft { get; set; }
}
