using Varjupaik.Services.Dtos;

namespace Varjupaik.Services.Notifications;

// Teenuste kiht kutsub seda pärast andmete salvestamist. Realisatsioon (WebSocket) on API projektis.
public interface IApplicationNotifier
{
    Task ApplicationReceivedAsync(ApplicationListDto application);
}
