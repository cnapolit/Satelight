using Comms.Common.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class InitResponseHandler(IPlayniteAPI playniteApi) : IRequestHandler<InitializeRequest, InitializeResponse>
{
    public async ValueTask<InitializeResponse> HandleAsync(InitializeRequest request, CancellationToken token)
        => new() { Path = playniteApi.Paths.ConfigurationPath, Port = 5156 };
}
