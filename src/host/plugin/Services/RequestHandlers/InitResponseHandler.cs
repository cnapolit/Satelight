using Comms.Common.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class InitResponseHandler(IPlayniteAPI playniteApi) : RequestHandler<InitializeRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(InitializeRequest request, CancellationToken token)
        => new InitializeResponse { Path = playniteApi.Paths.ConfigurationPath, Port = 5156 };
}
