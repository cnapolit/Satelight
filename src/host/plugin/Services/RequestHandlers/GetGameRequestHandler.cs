using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

internal class GetGameRequestHandler(IGetGamesService getGamesService) : RequestHandler<GetGameRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetGameRequest getGameRequest, CancellationToken _)
    {
        var game = getGamesService.GetGame(getGameRequest)
                ?? throw new ArgumentException($"Game {getGameRequest.Id} not found");
        return new GetGameResponse { Game = game };
    }
}
