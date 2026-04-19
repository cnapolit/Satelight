using Comms.Common.Interface.Models;
using Playnite;
using Common.Utility.Extensions;
using Game = Comms.Common.Interface.Models.Game;
using System.Collections.Immutable;
using Plugin.Common.Extensions;
using Plugin.Models;

namespace HostPlugin.Services;

public class GetGamesService(IPlayniteApi playniteApi) : IGetGamesService
{
    private static readonly string[] ExcludedTags = ["[EMT]", "[PS]"];

    public Game? GetGame(GameRequest gameRequest)
    {
        var game = playniteApi.Library.Games.Get(gameRequest.Id);
        return game is null ? null : GetGame(game, playniteApi.Library.GameRelations.Get, GetExcludedTagIds());
    }

    public IEnumerable<Game> GetGames()
    {
        var excludedTags = GetExcludedTagIds();
        var gameRelations = playniteApi.Library.GameRelations.ToList();
        var linkedGamesToExclude = gameRelations.SelectMany(r => r.LinkedGames).ToImmutableHashSet();

        return from game in playniteApi.Library.Games
               where linkedGamesToExclude.Contains(game.Id)
               select GetGame(game, i => gameRelations.FirstOrDefault(r => r.PrimaryGame == i), excludedTags);
    }

    private Game GetGame(
        Playnite.Game game, Func<string, GameRelation?> gameRelationFunc, IList<string> excludedTags)
    {
        var relation = gameRelationFunc(game.Id);
        var linkedGames = relation?.LinkedGames ?? [];
        List<Playnite.Game> games = [game];
        games.AddRange(linkedGames.Select(l => playniteApi.Library.Games.Get(l)!));
        return PlayniteToServerGame(games, excludedTags);
    }

    private List<string> GetExcludedTagIds() 
        => playniteApi.Library.Tags
            .Where(t => ExcludedTags.Any(t.Name.StartsWith))
            .Select(t => t.Id)
            .ToList();

    private GameInstance PlayniteToServerGameInstance(Playnite.Game copy)
    {
        var description = playniteApi.Library.GameDescriptions.Get(copy.Id);
        return new()
        {
            Id = copy.Id,
            Name = copy.Name,
            Description = description?.Text,
            ReleaseDate = copy.ReleaseDate?.Date.Ticks,
            Developers = copy.DeveloperIds.AsArray(),
            Publishers = copy.PublisherIds.AsArray(),
            Features = copy.FeatureIds.AsArray(),
            Platforms = copy.PlatformIds.AsArray(),
            LibraryId = copy.LibraryId,
            Installed = copy.InstallState is InstallState.Installed,
            Size = (long?)copy.InstallSize,
            Source = copy.SourceId,
            LibraryGameId = copy.LibraryGameId,
            PlayActions = copy.GameActionIds ?? [],
            Favorite = copy.Favorite
        };
    }

    private Game PlayniteToServerGame(IList<Playnite.Game> games, IList<string> excludedTagIds)
    {
        var primaryGame = games[0];
        var description = playniteApi.Library.GameDescriptions.Get(primaryGame.Id);
        var notes = playniteApi.Library.GameNotes.Get(primaryGame.Id);
        var userScore = games.Max(g => g.UserScore);
        var series = games
            .SelectMany(g => g.SeriesIds ?? [])
            .Distinct()
            .Select(s => playniteApi.Library.Series.Get(s)!.Name)
            .ToList();

        return new()
        {
            Id = primaryGame.Id,
            Name = primaryGame.Name,
            SortingName = primaryGame.SortingName,
            Favorite = primaryGame.Favorite || games.Any(g => g.Favorite),
            UserScore = games.Max(g => g.UserScore),
            CriticScore = games.Max(g => g.CriticScore),
            CommunityScore = games.Max(g => g.CommunityScore),
            Series = series,
            Instances = games.Select(PlayniteToServerGameInstance).ToList(),

            Cover = primaryGame.MediaFiles
                  ?.FirstOrDefault(f => f.GetMediaFileType() == MediaFileType.DesktopCover)?.Path,
            Background = primaryGame.MediaFiles
                  ?.FirstOrDefault(f => f.GetMediaFileType() == MediaFileType.DesktopBackground)?.Path,
            LastPlayed = primaryGame.LastPlayedDate?.Ticks,
            TimePlayed = primaryGame.PlayTime,
            Description = description?.Text,
            Notes = notes?.Text,
            Genres = primaryGame.GenreIds.AsArray(),
            Features = primaryGame.FeatureIds.AsArray(),
            Categories = primaryGame.CategoryIds.AsArray(),
            CompletionStatus = primaryGame.CompletionStatusId,
            Publishers = primaryGame.PublisherIds?.ToList() ?? [],
            Tags = primaryGame.TagIds?.Where(t => !excludedTagIds.Contains(t)).ToList() ?? [],
        };
    }
}