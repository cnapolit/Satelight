using Comms.Common.Interface.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

internal class GetGameRequestHandler(IGetGamesService getGamesService) : RequestHandler<GetGameRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetGameRequest getGameRequest, CancellationToken _)
        => new GetGameResponse { Game = getGamesService.GetGame(getGameRequest) };
}
