using Comms.Host.Interface;
using Comms.Host.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public interface IStopGameRequestHandler
{
    Task HandleRequestAsync(StopGameRequest request, IHostConnection connection, CancellationToken token);
}
