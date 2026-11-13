using System.Net.WebSockets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Varjupaik.Api.WebSockets;

public static class ApplicationsSocketEndpoint
{
    // WebSocket /ws/applications?access_token=<JWT>
    // Ühenduda saavad ainult Admin ja Staff. Brauseri WebSocket ei saa päiseid saata,
    // seepärast antakse token query-parameetris (vt JwtBearer OnMessageReceived Program.cs-is).
    public static void MapApplicationsSocket(this WebApplication app)
    {
        app.MapGet("/ws/applications", async (HttpContext context, WebSocketConnectionManager connections) =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            var user = context.User.FindFirstValue(ClaimTypes.Email) ?? "tundmatu";
            var id = connections.Add(socket, user);

            try
            {
                // Server ainult saadab; siin ootame, kuni klient ühenduse sulgeb
                var buffer = new byte[1024];
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer, context.RequestAborted);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (WebSocketException)
            {
            }
            finally
            {
                connections.Remove(id);
                if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
                }
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Staff" });
    }
}
