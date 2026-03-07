using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface;
using Comms.Host.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public interface IStopGameRequestHandler
{
    Task HandleRequestAsync(StopGameRequest request, ISatelightConnection connection, CancellationToken token);
}
