using Comms.Host.Interface;

namespace HostPlugin.Services.RequestHandlers;

public interface IRequestHandlerDirectory
{
    Task HandleAsync(IHostConnection connection, CancellationToken token);
}
