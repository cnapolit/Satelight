using Comms.Common.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

public class InitResponseHandler(IPlayniteApi playniteApi) : RequestHandler<InitializeRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(InitializeRequest request, CancellationToken token)
        => new InitializeResponse { Path = playniteApi.AppInfo.ConfigurationDirectory, Port = 5156 };
}
