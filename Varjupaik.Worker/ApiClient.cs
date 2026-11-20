using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Varjupaik.Worker;

// Kõik päringud API-le käivad läbi HttpClient-i, andmebaasi poole Worker ei pöördu
public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    // Logib sisse teenusekontoga ja jätab tokeni meelde järgmiste päringute jaoks
    public async Task LoginAsync(string email, string password, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", new { email, password }, ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResult>(ct);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.Token);
    }

    // GET /api/vaccinations/upcoming?days=7
    public async Task<List<UpcomingVaccination>> GetUpcomingVaccinationsAsync(int days, CancellationToken ct)
    {
        var result = await _http.GetFromJsonAsync<List<UpcomingVaccination>>($"/api/vaccinations/upcoming?days={days}", ct);
        return result ?? new List<UpcomingVaccination>();
    }

    // POST /api/notifications
    public async Task SendNotificationAsync(string title, string message, CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync("/api/notifications", new NotificationRequest { Title = title, Message = message }, ct);
        response.EnsureSuccessStatusCode();
    }
}
