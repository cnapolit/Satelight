using System.Linq.Expressions;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Server;
using Server.Database;
using Server.Models.Database;
using Game = Satelight.Protos.Server.Game;

namespace Server.Services.Protos;

public class FiltersService(
    ILogger<GamesService> logger, IDbContextFactory<DatabaseContext> databaseContextFactory) : Filters.FiltersBase
{
    public override Task<ListFiltersReply> List(ListFiltersBody request, ServerCallContext context) => base.List(request, context);

    //public Task Create(IEnumerable<FilterAspect> filters)
    //{
    //    foreach (var filter in filters)
    //    {
    //        switch (filter.Field)
    //        {
    //            case nameof(Models.Game.Name):                                     break;
    //            case nameof(Models.Game.Description):                              break;
    //            case nameof(Models.UserGameInfo.CompletionStatus):                 break;
    //            case nameof(Models.UserGameInfo.Favorite):                         break;
    //            case nameof(Models.UserGameInfo.LastActivity):                     break;
    //            case nameof(Models.UserGameInfo.PlayTime):                         break;
    //            case nameof(Models.UserGameInfo.PlayCount):                        break;
    //            case nameof(Models.UserGameInfo.Score):                            break;
    //            case nameof(User):                                                 break;
    //            case nameof(Account):                                              break;
    //            case nameof(Library):                                              break;
    //            case nameof(GameVariant) + "." + nameof(GameVariant.Description):  break;
    //            case nameof(GameVariant) + "." + nameof(GameVariant.Name):         break;
    //            case nameof(GameVariant) + "." + nameof(GameVariant.Publishers):   break;
    //            case nameof(GameVariant) + "." + nameof(GameVariant.Platforms):    break;
    //            case nameof(Models.Host) + "." + nameof(Models.HostGame.Size):     break;
    //            case nameof(Models.Host):                                          break;
    //            case nameof(Models.HostGame) + "." + nameof(Models.HostGame.Size): break;
    //        }
    //    }
    //}

    public async IAsyncEnumerable<Models.Database.Game> FilterAsync(
        StreamGamesBody streamGamesBody,
        IServerStreamWriter<Game> responseStream,
        ServerCallContext context,
        CustomFilter filter)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        var gameQuery = databaseContext
                       .Games
                       .AsNoTracking()
                       .Include(g => g.GameVariants)
                       .ThenInclude(v => v.LibraryGames)
                       .ThenInclude(l => l.AccountOwners)
                       .ThenInclude(a => a.Account)
                       .Where(g => g.GameVariants.Any(
                                  v => v.LibraryGames.Any(
                                      l => l.AccountOwners.Any(
                                          a => a.Account.UserId == filter.UserId))));

        IQueryable<Models.Database.Game> games = gameQuery;
        //if (FieldIsPresent(filter, nameof(Models.Game.Genres)))
        //{
        //    games = games.Include(g => g.Genres);
        //}

        foreach (var aspect in filter.Aspects)
        {
            switch (aspect.Field)
            {
                case nameof(Models.Database.Game.Name):
                    HandleString(ref games, aspect, g => g.Name);
                    break;
                case nameof(Models.Database.Game.Description):
                    HandleString(ref games, aspect, g => g.Description); break;
                case nameof(Models.Database.UserGameInfo.CompletionStatus):                              break;
                case nameof(Models.Database.UserGameInfo.Favorite):                                      break;
                case nameof(Models.Database.UserGameInfo.LastActivity):                                  break;
                case nameof(Models.Database.UserGameInfo.PlayTime):                                      break;
                case nameof(Models.Database.UserGameInfo.PlayCount):                                     break;
                case nameof(Models.Database.UserGameInfo.Score):                 
                    HandleNullable(ref games, aspect, short.Parse, g => g.UserGameInfo.First(i => i.UserId == filter.UserId).Score);
                    break;
                case nameof(User):                                                       break;
                case nameof(Account):                                                    break;
                case nameof(Library):                                                    break;
                //case nameof(GameVariant) + "." + nameof(GameVariant.Name):        break;
                //case nameof(GameVariant) + "." + nameof(GameVariant.Description): break;
                case nameof(Models.Database.HostGame)    + "." + nameof(Models.Database.HostGame.Size):           break;
                case nameof(Models.Database.Host):                                                       break;
            }

        }
        

        var gamesEnumerable = games.AsAsyncEnumerable().WithCancellation(context.CancellationToken);
        await foreach (var game in gamesEnumerable)
        {
            yield return null;
        }
    }

    private static void HandleString(
        ref IQueryable<Models.Database.Game> games, FilterAspect aspect, Expression<Func<Models.Database.Game, string>> strGetter)
    {
        var value = Expression.Constant(aspect.Value);
        var expression = aspect.Operator switch
        {
            Operator.Contains   => GetExpression(nameof(string.Contains),   strGetter, value),
            Operator.StartsWith => GetExpression(nameof(string.StartsWith), strGetter, value),
            Operator.EndsWith   => GetExpression(nameof(string.EndsWith),   strGetter, value),
            _                   => Expression.Equal(strGetter.Body, value)
        };
        var lambda = Expression.Lambda<Func<Models.Database.Game, bool>>(expression, strGetter.Parameters);
        games = games.Where(lambda);
    }

    private static void Handle<T>(
        ref IQueryable<Models.Database.Game> games, FilterAspect aspect, Func<string, T> valueParser, Expression<Func<Models.Database.Game, T>> intGetter)
    {
        var value = Expression.Constant(valueParser(aspect.Value));
        var expression = aspect.Operator switch
        {
            Operator.LessThan    => Expression.LessThan(intGetter.Body, value),
            Operator.GreaterThan => Expression.GreaterThan(intGetter.Body, value),
            _                    => Expression.Equal(intGetter.Body, value)
        };
        var lambda = Expression.Lambda<Func<Models.Database.Game, bool>>(expression, intGetter.Parameters);
        games = games.Where(lambda);
    }
    private static void HandleNullable<T>(
        ref IQueryable<Models.Database.Game> games,
        FilterAspect aspect, Func<string, T> valueParser,
        Expression<Func<Models.Database.Game, T?>> intGetter) where T : struct
    {
        var value = Expression.Constant((T?)valueParser(aspect.Value));
        var expression = aspect.Operator switch
        {
            Operator.LessThan    => Expression.LessThan(intGetter.Body, value),
            Operator.GreaterThan => Expression.GreaterThan(intGetter.Body, value),
            _                    => Expression.Equal(intGetter.Body, value)
        };
        var lambda = Expression.Lambda<Func<Models.Database.Game, bool>>(expression, intGetter.Parameters);
        games = games.Where(lambda);
    }

    private static Expression GetExpression<T>(
        string methodName, Expression<Func<Models.Database.Game, T>> strGetter, ConstantExpression valueExpression)
        => Expression.Call(strGetter.Body, typeof(T).GetMethod(methodName, [typeof(T)])!, valueExpression);

    private static bool FieldIsPresent(CustomFilter filter, string fieldName)
        => filter.Aspects.Any(a => a.Field.StartsWith(fieldName))
        || filter.Sort              .Field.StartsWith(fieldName)
        || filter.Grouping          .Field.StartsWith(fieldName);
}


public class CustomFilter
{
    public Guid               UserId   { get; set; }
    public List<FilterAspect> Aspects  { get; set; } = [];
    public FilterSort         Sort     { get; set; }
    public FilterGrouping     Grouping { get; set; }
}

public class FilterAspect
{
    public string          Field           { get; set; } = string.Empty;
    public string          Value           { get; set; } = string.Empty;
    public bool            Not             { get; set; }
    public Operator        Operator        { get; set; }
}

public class FilterSort
{
    public string Field      { get; set; } = string.Empty;
    public bool   Descending { get; set; }
}
public class FilterGrouping
{
    public string Field { get; set; } = string.Empty;
    public FilterSort Sort { get; set; }
}

public enum Operator
{
    Unknown,
    Equals,
    GreaterThan,
    LessThan,
    Contains,
    StartsWith,
    EndsWith
}