using Comms.Common.Interface.Models;
using Playnite.SDK;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class CountGamesHandler(IPlayniteAPI playniteApi) : IRequestHandler<CountGamesRequest, CountGamesResponse>
{
    public async ValueTask<CountGamesResponse> HandleAsync(CountGamesRequest request, CancellationToken token)
    {
        return new() { Count = playniteApi.Database.GetFilteredGames(new() { Hidden = false }).Count() };
    }
}
