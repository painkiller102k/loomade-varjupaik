namespace Varjupaik.Services.Models;

// Vaktsineerimine; NextDueDate näitab, millal tuleb järgmine kord vaktsineerida
public class Vaccination
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public Animal? Animal { get; set; }
    public string Name { get; set; } = "";
    public DateTime VaccinationDate { get; set; }
    public DateTime? NextDueDate { get; set; }
}
