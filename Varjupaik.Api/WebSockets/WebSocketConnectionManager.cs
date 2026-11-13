using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Varjupaik.Api.WebSockets;

// Hoiab kõiki avatud WebSocket ühendusi (ainult töötajad) ja saadab neile sõnumeid
public class WebSocketConnectionManager
{
    private class Connection
    {
        public required WebSocket Socket { get; init; }
        public required string User { get; init; }
        public SemaphoreSlim SendLock { get; } = new(1, 1);
    }

    private readonly ConcurrentDictionary<Guid, Connection> _connections = new();
    private readonly ILogger<WebSocketConnectionManager> _logger;

    public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger)
    {
        _logger = logger;
    }

    public int Count => _connections.Count;

    public Guid Add(WebSocket socket, string user)
    {
        var id = Guid.NewGuid();
        _connections[id] = new Connection { Socket = socket, User = user };
        _logger.LogInformation("WebSocket ühendus avati: {User}, ühendusi kokku {Count}", user, _connections.Count);
        return id;
    }

    public void Remove(Guid id)
    {
        if (_connections.TryRemove(id, out var connection))
        {
            _logger.LogInformation("WebSocket ühendus suleti: {User}, ühendusi kokku {Count}", connection.User, _connections.Count);
        }
    }

    // Saadab sõnumi kõigile avatud ühendustele
    public async Task BroadcastAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);

        foreach (var (id, connection) in _connections)
        {
            await connection.SendLock.WaitAsync();
            try
            {
                if (connection.Socket.State == WebSocketState.Open)
                {
                    await connection.Socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                // Katkenud ühendus ei tohi teisi töötajaid segada
                _logger.LogWarning(ex, "Sõnumi saatmine ebaõnnestus ({User}), ühendus eemaldati", connection.User);
                Remove(id);
            }
            finally
            {
                connection.SendLock.Release();
            }
        }
    }
}
