using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Satelight.Protos.Server;
using Server.Database;
using Common.Utility.Extensions;
using Server.Services.Host;
using Game = Satelight.Protos.Server.Game;
using GameVariant = Satelight.Protos.Server.GameVariant;
using HostGame = Satelight.Protos.Server.HostGame;
using LibraryGame = Satelight.Protos.Server.LibraryGame;

namespace Server.Services.Protos;

public class GamesService(
    ILogger<GamesService> logger,
    IDbContextFactory<DatabaseContext> databaseContextFactory,
    HostClient hostClient,
    EventsService eventsService) : Games.GamesBase
{
    public override async Task<GetGameReply> Get(GetGameBody body, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        var gameId = body.GameId.ToGuid();
        var game = await databaseContext
                        .Games
                        .Include    (g => g.GameVariants)
                        .ThenInclude(v => v.LibraryGames)
                        .ThenInclude(g => g.HostGames)
                        .FirstOrDefaultAsync(g => g.Id == gameId, context.CancellationToken)
                ?? throw new RpcException(new(StatusCode.NotFound, $"Game '{gameId}' does not exist"));
        return new() { Game = GetGame(game) };
    }

    public override async Task<InstallGameReply> Install(InstallGameBody body, ServerCallContext context)
        => new() { Op = await TriggerActionAsync(hostClient.InstallAsync, body.UserGameAction, context.CancellationToken) };

    public override async Task<UninstallGameReply> Uninstall(UninstallGameBody body, ServerCallContext context)
        => new() { Op = await TriggerActionAsync(hostClient.UninstallAsync, body.UserGameAction, context.CancellationToken) };

    public override async Task<RemoveGameReply> Remove(RemoveGameBody body, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        var game = await databaseContext.Games.FindAsync(body.GameId.ToGuid())
                ?? throw new RpcException(new(StatusCode.NotFound, $"Game '{body.GameId.ToGuid()}' does not exist"));
        databaseContext.Games.Remove(game);
        await databaseContext.SaveChangesAsync(context.CancellationToken);
        return new() { Result = new() { Success = true } };
    }

    public override async Task<RepairGameReply> Repair(RepairGameBody body, ServerCallContext context)
        => new() { Op = await TriggerActionAsync(hostClient.RepairAsync, body.UserGameAction, context.CancellationToken) };

    public override async Task<StartGameReply> Start(StartGameBody body, ServerCallContext context)
        => new() { Op = await TriggerActionAsync(hostClient.StartAsync, body.UserGameAction, context.CancellationToken) };

    public override async Task<StopGameReply> Stop(StopGameBody body, ServerCallContext context)
        => new() { Op = await TriggerActionAsync(hostClient.StopAsync, body.UserGameAction, context.CancellationToken) };


    public override async Task Stream(
        StreamGamesBody streamGamesBody, IServerStreamWriter<Game> responseStream, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        var games = databaseContext
                   .Games
                   .AsNoTracking()
                   .Include    (g => g.GameVariants)
                   .ThenInclude(v => v.LibraryGames)
                   .ThenInclude(g => g.HostGames)
                   .AsAsyncEnumerable()
                   .WithCancellation(context.CancellationToken);
        await foreach (var game in games) 
        await responseStream.WriteAsync(GetGame(game), context.CancellationToken);
    }

    private static Game GetGame(Models.Database.Game game)
    {
        Game responseGame = new()
        {
            Id          = game.Id.ToByteString(),
            ReleaseDate = 0,
            DateAdded   = 0,
            Name        = game.Name,
            Description = game.Description,
            SortingName = game.SortingName
        };

        foreach (var gameVariant in game.GameVariants)
        {
            GameVariant responseVariant = new()
            {
                Id          = gameVariant.Id.ToByteString(),
                Name        = gameVariant.Name,
                SubName     = gameVariant.SubName,
                Description = gameVariant.Description
            };

            ProtoMapper.AddIds(responseVariant.Platforms,  gameVariant.Platforms);
            ProtoMapper.AddIds(responseVariant.Publishers, gameVariant.Publishers);
            ProtoMapper.AddIds(responseVariant.Developers, gameVariant.Developers);
            ProtoMapper.AddIds(responseVariant.Features,   gameVariant.Features);

            foreach (var libraryGame in gameVariant.LibraryGames)
            {
                LibraryGame libraryGameInstance = new();
                foreach (var hostGames in libraryGame.HostGames.GroupBy(h => h.HostId))
                {
                    HostState hostState = new() { HostId = hostGames.Key.ToByteString() };
                    hostState.Games.AddRange(hostGames.Select(ConvertToHostGame));
                    libraryGameInstance.HostStates.Add(hostState);
                }
                responseVariant.LibraryGames.Add(libraryGameInstance);
            }
            responseGame.GameVariants.Add(responseVariant);
        }

        return responseGame;
    }

    private static Task<Op> TriggerActionAsync(
        Func<Guid, Guid, Guid, CancellationToken, Task<Op>> action,
        UserGameAction userGameAction,
        CancellationToken cancellationToken)
    {
        var gameId    = userGameAction.GameIdentifier.LibraryGameId.ToGuid();
        var libraryId = userGameAction.GameIdentifier.LibraryId    .ToGuid();
        var hostId    = userGameAction.GameIdentifier.HostId       .ToGuid();
        return action(hostId, libraryId, gameId, cancellationToken);
    }
    private static HostGame ConvertToHostGame(Models.Database.HostGame hostGame) => new()
    {
        Id        = hostGame.Id.ToByteString(),
        Installed = hostGame.Installed,
        Playing   = hostGame.Playing,
        Size      = hostGame.Size ?? 0
    };

    private static Models.Database.HostGame ConvertToHostGame(Satelight.Protos.Host.HostGame hostGame, Models.Database.LibraryGame libraryGame) => new()
    {
        HostGameId = hostGame.Id.ToGuid().ToString(),
        LibraryGame = libraryGame,
        Installed   = hostGame.Installed,
        Playing     = hostGame.Playing,
        Size = hostGame.Size is 0 ? null : hostGame.Size,
    };
}
