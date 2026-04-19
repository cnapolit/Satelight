using Comms.Common.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

public class CountGamesHandler(IPlayniteApi playniteApi) : RequestHandler<CountGamesRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(CountGamesRequest request, CancellationToken token)
    {
        var linkedGameCount = playniteApi.Library.GameRelations.Sum(r => r.LinkedGames.Count);
        var total = playniteApi.Library.Games.Count - linkedGameCount;
        return new CountGamesResponse { Count = total };
    }
}
