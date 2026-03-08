using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility.Extensions;
using Game = Comms.Common.Interface.Models.Game;

namespace HostPlugin.Services;

public class GetGamesService(IPlayniteAPI playniteApi) : IGetGamesService
{

    private const           string   DuplicateHiderTagPrefix = "[DH]";
    private static readonly string[] ExcludedTags            = [DuplicateHiderTagPrefix, "[EMT]", "[PS]"];

    public Playnite.SDK.Models.Game GetGame(HostGameRequest game)
    {
        if (game.Id != Guid.Empty)
        {
            return playniteApi.Database.Games.Get(game.Id);
        }

        return playniteApi.Database.Games
            .First(g => g.PluginId == game.PluginId && g.GameId == game.PluginGameId);
    }

    public Game GetGame(GameRequest game)
        => PlayniteToServerGame(playniteApi.Database.Games.Get(game.Id));

    public IEnumerable<Game> GetGames() => GetFilteredGames(new() { Hidden = false });

    public IEnumerable<Game> GetGames(Guid[] pluginIds)
        => GetGames(playniteApi.Database.Games.Where(g => !g.Hidden && pluginIds.Contains(g.PluginId)));

    public IEnumerable<Game> GetGamesFromSource(Guid sourceId)
        => GetFilteredGames(new() { Hidden = false, Source = new() { Ids = [sourceId] } });

    public IEnumerable<Game> GetGamesFromLibrary(Guid libraryId)
        => GetFilteredGames(new() { Hidden = false, Library = new() { Ids = [libraryId] } });

    public IEnumerable<Game> GetFilteredGames(FilterPresetSettings filterSettings)
        => GetGames(playniteApi.Database.GetFilteredGames(filterSettings));

    private IEnumerable<Game> GetGames(IEnumerable<Playnite.SDK.Models.Game> games)
        => games.Select(PlayniteToServerGame);

    private IEnumerable<GameInstance> GetGameInstances(Playnite.SDK.Models.Game game)
    {
        var duplicateTag = game.Tags.FirstOrDefault(t => t.Name.StartsWith(DuplicateHiderTagPrefix));
        return duplicateTag is null
            ? [PlayniteToServerGameInstance(game)]
            : playniteApi
             .Database
             .GetFilteredGames(new() { Tag = new() { Ids = [duplicateTag.Id] } })
             .Select(PlayniteToServerGameInstance);
    }

    private static GameInstance PlayniteToServerGameInstance(Playnite.SDK.Models.Game copy) => new()
    {
        Id           = copy.Id,
        Name         = copy.Name,
        Description  = copy.Description,
        ReleaseDate  = copy.ReleaseDate?.Date.Ticks,
        Developers   = copy.DeveloperIds.AsArray(),
        Publishers   = copy.PublisherIds.AsArray(),
        Features     = copy.  FeatureIds.AsArray(),
        Platforms    = copy. PlatformIds.AsArray(),
        LibraryId    = copy.PluginId,
        Installed    = copy.IsInstalled,
        Size         = (long?)copy.InstallSize,
        Source       = copy.Source?.Id ?? Guid.Empty,
        Playing      = copy.IsRunning,
        PluginId     = copy.PluginId,
        PluginGameId = copy.GameId,
        PlayActions  = copy.GameActions?.Select(a => a.Name).ToArray() ?? [],
        Favorite     = copy.Favorite
    };

    private Game PlayniteToServerGame(Playnite.SDK.Models.Game game) => new()
    {
        Id               = game.Id,
        Name             = game.Name,
        Description      = game.Description,
        Notes            = game.Notes,
        LastPlayed       = game.LastActivity?.Ticks,
        TimePlayed       = (long)game.Playtime,
        Favorite         = game.Favorite,
        Cover            = game.CoverImage,
        Background       = game.BackgroundImage,
        CompletionStatus = game.CompletionStatus.Id,
        Publisher        = game.Publishers?.FirstOrDefault()?.Name ?? string.Empty,
        UserScore        = game.     UserScore.AsValue(),
        CriticScore      = game.   CriticScore.AsValue(),
        CommunityScore   = game.CommunityScore.AsValue(),
        Genres           = game.   GenreIds.AsArray(),
        Features         = game. FeatureIds.AsArray(),
        Categories       = game.CategoryIds.AsArray(),
        SortingName      = game.SortingName,
        PlayCount        = (long)game.PlayCount,
        Series           = game.Series?.Select(s => s.Name).ToArray() ?? [],
        Tags             = [..FilterOutPluginTags(game)],
        Instances        = [..GetGameInstances(game)]
    };

    private static IEnumerable<Guid> FilterOutPluginTags(Playnite.SDK.Models.Game game)
        => from   tag in game.Tags
           where  !ExcludedTags.Any(tag.Name.StartsWith)
           select tag.Id;
}