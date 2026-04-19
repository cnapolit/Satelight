using Comms.Common.Interface.Models;
using Comms.Host.Interface;

namespace HostPlugin.Services.RequestHandlers;

public interface IGamesStreamHandler
{
    Task HandleRequestAsync(StreamGamesRequest request, IHostConnection connection, CancellationToken token);
}
