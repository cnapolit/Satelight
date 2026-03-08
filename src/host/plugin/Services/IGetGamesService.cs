using System;
using System.Collections.Generic;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK.Models;
using Game = Comms.Common.Interface.Models.Game;

namespace HostPlugin.Services;

public interface IGetGamesService
{
    Playnite.SDK.Models.Game GetGame(HostGameRequest game);
    Game GetGame(GameRequest game);
    IEnumerable<Game> GetGames();
    IEnumerable<Game> GetGames(Guid[] pluginIds);
    IEnumerable<Game> GetGamesFromSource(Guid sourceId);
    IEnumerable<Game> GetGamesFromLibrary(Guid libraryId);
    IEnumerable<Game> GetFilteredGames(FilterPresetSettings filterSettings);
}
