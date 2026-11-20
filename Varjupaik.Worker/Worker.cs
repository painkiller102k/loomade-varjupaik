using Microsoft.Extensions.Options;

namespace Varjupaik.Worker;

// Iga päev kell 8.00 (seadistatav) küsib API-lt lähenevad vaktsineerimised ja saadab töötajatele meeldetuletused.
// Äriloogika (mis on "lähenev") on API-s, Worker ainult käivitab selle.
public class Worker : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiOptions _api;
    private readonly WorkerOptions _options;
    private readonly ILogger<Worker> _logger;

    public Worker(IHttpClientFactory httpClientFactory, IOptions<ApiOptions> api, IOptions<WorkerOptions> options, ILogger<Worker> logger)
    {
        _httpClientFactory = httpClientFactory;
        _api = api.Value;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var nextRun = GetNextRun(DateTime.Now);
        _logger.LogInformation("Taustateenus käivitus. Järgmine töö: {NextRun}", nextRun);

        if (_options.RunOnStartup)
        {
            await RunJobAsync(stoppingToken);
        }

        // PeriodicTimer annab märku iga paari sekundi tagant, kontrollime, kas on aeg tööd teha
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.CheckIntervalSeconds));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (DateTime.Now >= nextRun)
            {
                await RunJobAsync(stoppingToken);
                nextRun = GetNextRun(DateTime.Now);
                _logger.LogInformation("Järgmine töö: {NextRun}", nextRun);
            }
        }
    }

    // Järgmine kellaaeg (nt 08:00): täna, kui see pole veel möödas, muidu homme
    private DateTime GetNextRun(DateTime now)
    {
        var today = new DateTime(now.Year, now.Month, now.Day, _options.RunAtHour, _options.RunAtMinute, 0);
        return today > now ? today : today.AddDays(1);
    }

    private async Task RunJobAsync(CancellationToken ct)
    {
        _logger.LogInformation("Alustan vaktsineerimiste kontrolli");

        try
        {
            var client = new ApiClient(_httpClientFactory.CreateClient("api"));
            await client.LoginAsync(_api.Email, _api.Password, ct);

            var upcoming = await client.GetUpcomingVaccinationsAsync(_options.Days, ct);
            if (upcoming.Count == 0)
            {
                _logger.LogInformation("Järgmise {Days} päeva jooksul vaktsineerimisi pole", _options.Days);
                return;
            }

            foreach (var vaccination in upcoming)
            {
                var (title, message) = BuildMessage(vaccination);
                await client.SendNotificationAsync(title, message, ct);
            }

            _logger.LogInformation("Saadetud {Count} meeldetuletust töötajatele", upcoming.Count);
        }
        catch (OperationCanceledException)
        {
            // teenus peatati
        }
        catch (Exception ex)
        {
            // Viga ei tohi teenust peatada, proovime uuesti järgmisel korral
            _logger.LogError(ex, "Vaktsineerimiste kontroll ebaõnnestus");
        }
    }

    private static (string Title, string Message) BuildMessage(UpcomingVaccination v)
    {
        var date = v.NextDueDate.ToString("dd.MM.yyyy");

        if (v.DaysLeft < 0)
        {
            return ("Vaktsineerimine on hilinenud", $"{v.AnimalName}: {v.Name} oli vaja teha {date}, hilinemine {-v.DaysLeft} päeva.");
        }

        if (v.DaysLeft == 0)
        {
            return ("Lähenev vaktsineerimine", $"{v.AnimalName}: {v.Name} tuleb teha täna ({date}).");
        }

        return ("Lähenev vaktsineerimine", $"{v.AnimalName}: {v.Name} tuleb teha {v.DaysLeft} päeva pärast ({date}).");
    }
}
