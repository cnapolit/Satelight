using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;

namespace HostPlugin.Services.RequestHandlers;

public class RemoveGameHandler
{
    public RemoveGameResponse Handle(
        IPlayniteAPI playniteApi, GetGamesService getGamesService, RemoveGameRequest request)
    {
        var game = getGamesService.GetGame(request);
        var wasRemoved = playniteApi.Database.Games.Remove(game);
        return new() { Success = wasRemoved };
    }
}