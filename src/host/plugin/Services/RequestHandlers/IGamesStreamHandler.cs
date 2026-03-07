using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface;
using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public interface IGamesStreamHandler
{
    Task HandleRequestAsync(StreamGamesRequest request, ISatelightConnection connection, CancellationToken token);
}
