using Comms.Common.Interface.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

internal class GetGameRequestHandler(GetGamesService getGamesService)
    : IRequestHandler<GetGameRequest, GetGameResponse>
{
    public async ValueTask<GetGameResponse> HandleAsync(GetGameRequest getGameRequest, CancellationToken _)
        => new() { Game = getGamesService.GetGame(getGameRequest) };
}
