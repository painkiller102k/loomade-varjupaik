namespace Varjupaik.Services.Models;

// Teavitus töötajatele (nt meeldetuletus lähenevast vaktsineerimisest)
public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
