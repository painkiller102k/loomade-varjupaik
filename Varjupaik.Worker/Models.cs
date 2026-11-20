namespace Varjupaik.Worker;

// Worker ei viita teistele projektidele ega andmebaasile, seepärast on siin oma lihtsad klassid API vastuste jaoks

public class LoginResult
{
    public string Token { get; set; } = "";
}

public class UpcomingVaccination
{
    public int AnimalId { get; set; }
    public string AnimalName { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime NextDueDate { get; set; }
    public int DaysLeft { get; set; }
}

public class NotificationRequest
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
}
