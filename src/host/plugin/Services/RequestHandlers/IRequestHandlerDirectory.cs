using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface;

namespace HostPlugin.Services.RequestHandlers;

public interface IRequestHandlerDirectory
{
    Task HandleAsync(ISatelightConnection connection, CancellationToken token);
}
