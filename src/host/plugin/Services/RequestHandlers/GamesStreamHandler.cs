using System.Threading;
using System.Threading.Tasks;
using Common.Utility.Extensions;
using Comms.Common.Interface;
using Comms.Common.Interface.Models;
using HostPlugin.Services;

namespace HostPlugin.Services.RequestHandlers;

public class GamesStreamHandler(IGetGamesService getGamesService) : IGamesStreamHandler
{
    public Task HandleRequestAsync(StreamGamesRequest request, ISatelightConnection connection, CancellationToken token)
        => getGamesService
          .GetGames()
          .While(connection.IsConnected)
          .WithCancellation(token)
          .ForEachAsync(g => connection.SendResponseAsync(new StreamGamesResponse { Game = g }, token));
}