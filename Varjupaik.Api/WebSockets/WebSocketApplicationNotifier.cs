using System.Text.Json;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Notifications;

namespace Varjupaik.Api.WebSockets;

// Saadab uue taotluse kõigile ühendatud töötajatele sõnumina ApplicationReceived
public class WebSocketApplicationNotifier : IApplicationNotifier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly WebSocketConnectionManager _connections;

    public WebSocketApplicationNotifier(WebSocketConnectionManager connections)
    {
        _connections = connections;
    }

    public async Task ApplicationReceivedAsync(ApplicationListDto application)
    {
        var message = JsonSerializer.Serialize(new
        {
            type = "ApplicationReceived",
            data = application,
            sentAt = DateTime.UtcNow
        }, JsonOptions);

        await _connections.BroadcastAsync(message);
    }
}
