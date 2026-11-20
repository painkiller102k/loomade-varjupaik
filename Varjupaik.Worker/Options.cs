namespace Varjupaik.Worker;

// Seaded failist appsettings.json, jaotis "Api"
public class ApiOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5156";

    // Taustateenuse konto (roll Service)
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

// Seaded failist appsettings.json, jaotis "Worker"
public class WorkerOptions
{
    // Mis kell iga päev töö käivitub (kohalik aeg)
    public int RunAtHour { get; set; } = 8;
    public int RunAtMinute { get; set; } = 0;

    // Mitu päeva ette lähenevaid vaktsineerimisi vaadata
    public int Days { get; set; } = 7;

    // Kui true, siis töö käivitub kohe teenuse käivitamisel (testimiseks)
    public bool RunOnStartup { get; set; } = false;

    // Kui tihti kontrollime, kas on käivitamise aeg
    public int CheckIntervalSeconds { get; set; } = 30;
}
