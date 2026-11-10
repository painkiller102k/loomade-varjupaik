namespace Varjupaik.Services.Models;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Species { get; set; } = "";
    public int Age { get; set; }
    public string Description { get; set; } = "";
    public string? PhotoPath { get; set; }
    public AnimalStatus Status { get; set; } = AnimalStatus.Available;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
