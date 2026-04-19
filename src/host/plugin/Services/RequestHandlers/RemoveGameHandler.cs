using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

public class RemoveGameHandler(IPlayniteApi playniteApi) : RequestHandler<RemoveGameRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(RemoveGameRequest request, CancellationToken _)
    {
        var removedGame = await playniteApi.Library.Games.RemoveAsync(request.Id);
        return new RemoveGameResponse { Success = removedGame != null };
    }
}