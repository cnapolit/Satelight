using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class RemoveGameHandler(IPlayniteAPI playniteApi, IGetGamesService getGamesService) : RequestHandler<RemoveGameRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(RemoveGameRequest request, CancellationToken _)
    {
        var game = getGamesService.GetGame(request);
        var wasRemoved = playniteApi.Database.Games.Remove(game);
        return new RemoveGameResponse { Success = wasRemoved };
    }
}