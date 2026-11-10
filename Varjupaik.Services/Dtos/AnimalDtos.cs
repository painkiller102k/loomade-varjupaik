using System.ComponentModel.DataAnnotations;
using Varjupaik.Services.Models;

namespace Varjupaik.Services.Dtos;

public class AnimalDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Species { get; set; } = "";
    public int Age { get; set; }
    public string Description { get; set; } = "";
    public string? PhotoUrl { get; set; }
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public static AnimalDto FromAnimal(Animal a)
    {
        return new AnimalDto
        {
            Id = a.Id,
            Name = a.Name,
            Species = a.Species,
            Age = a.Age,
            Description = a.Description,
            PhotoUrl = a.PhotoPath,
            Status = a.Status.ToString(),
            CreatedAt = a.CreatedAt
        };
    }
}

public class AnimalSaveDto
{
    /// <example>Muri</example>
    [Required, MaxLength(50)]
    public string Name { get; set; } = "";

    /// <example>Koer</example>
    [Required, MaxLength(30)]
    public string Species { get; set; } = "";

    /// <summary>Vanus aastates</summary>
    /// <example>3</example>
    [Range(0, 50)]
    public int Age { get; set; }

    /// <example>Sõbralik koer, kes armastab jalutamist.</example>
    [MaxLength(1000)]
    public string Description { get; set; } = "";

    /// <summary>Available, Reserved või Adopted</summary>
    /// <example>Available</example>
    public AnimalStatus Status { get; set; } = AnimalStatus.Available;
}
