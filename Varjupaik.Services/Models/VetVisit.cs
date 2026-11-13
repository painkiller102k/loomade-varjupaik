namespace Varjupaik.Services.Models;

// Veterinaarivisiit: üks kord, kui loom käis arsti juures
public class VetVisit
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public Animal? Animal { get; set; }
    public DateTime VisitDate { get; set; }
    public string Reason { get; set; } = "";
    public string Notes { get; set; } = "";
    public string Vet { get; set; } = "";
}
