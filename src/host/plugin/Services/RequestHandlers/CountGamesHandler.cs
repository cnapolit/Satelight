using Comms.Common.Interface.Models;
using Playnite.SDK;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class CountGamesHandler(IPlayniteAPI playniteApi) : RequestHandler<CountGamesRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(CountGamesRequest request, CancellationToken token)
    {
        return new CountGamesResponse { Count = playniteApi.Database.GetFilteredGames(new() { Hidden = false }).Count() };
    }
}
