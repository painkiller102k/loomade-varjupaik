namespace Varjupaik.Services.Models;

public class Application
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public Animal? Animal { get; set; }
    public int ApplicantUserId { get; set; }
    public User? ApplicantUser { get; set; }
    public string Message { get; set; } = "";
    public ApplicationStatus Status { get; set; } = ApplicationStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
