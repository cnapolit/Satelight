using Comms.Common.Interface.Models;
using Game = Comms.Common.Interface.Models.Game;

namespace HostPlugin.Services;

public interface IGetGamesService
{
    Game? GetGame(GameRequest game);
    IEnumerable<Game> GetGames();
}
