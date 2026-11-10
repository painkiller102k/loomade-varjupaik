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
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Species { get; set; } = "";

    [Range(0, 50)]
    public int Age { get; set; }

    public string Description { get; set; } = "";

    public AnimalStatus Status { get; set; } = AnimalStatus.Available;
}
