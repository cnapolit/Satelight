using Common.Utility.Extensions;
using Google.Protobuf;
using Grpc.Core;
using Satelight.Protos.Core;
using Satelight.Protos.Host;
using HostGame = Server.Models.Database.HostGame;

namespace Server.Services.Host;

public class HostClient(ChannelManager channelManager)
{

    public async Task<Satelight.Protos.Host.HostGame?> GetGameAsync(Models.Database.Host host, HostGame hostGame, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Games.GamesClient gamesClient = new(channel);
        var hostGameIdByteStr = Guid.Parse(hostGame.HostGameId).ToByteString();
        var reply = await gamesClient.GetAsync(new()
        {
            Library = hostGame.LibraryGame.Library.Name,
            LibraryGameId = hostGame.LibraryGame.Id.ToString(),
            GameId = hostGameIdByteStr,
        }, cancellationToken: token);
        return reply.Game?.HostGames?.FirstOrDefault(h => h.Id == hostGameIdByteStr);
    }

    public async Task<int> GetGameCountAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Games.GamesClient gamesClient = new(channel);
        var gamesCountReply = await gamesClient.CountAsync(new(), cancellationToken: token);
        return gamesCountReply.Count;
    }

    public IAsyncEnumerable<Game> GetGamesAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Games.GamesClient gamesClient = new(channel);
        var gameStream = gamesClient.Stream(new(), cancellationToken: token);
        return gameStream.ResponseStream.ReadAllAsync(token);
    }

    public async Task<GetCacheReply> GetCacheAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Satelight.Protos.Core.Database.DatabaseClient databaseClient = new(channel);
        return await databaseClient.GetCacheAsync(new(), cancellationToken: token);
    }

    public async Task<IList<Label>> ListSeriesAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Series.SeriesClient seriesClient = new(channel);
        var reply = await seriesClient.ListAsync(new(), cancellationToken: token);
        return reply.Labels;
    }

    public async Task<IList<Label>> ListCompaniesAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Companies.CompaniesClient CompaniesClient = new(channel);
        var reply = await CompaniesClient.ListAsync(new(), cancellationToken: token);
        return reply.Labels;
    }

    public async Task<IList<Label>> ListFeaturesAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Features.FeaturesClient featuresClient = new(channel);
        var reply = await featuresClient.ListAsync(new(), cancellationToken: token);
        return reply.Labels;
    }

    public async Task<IList<Label>> ListLibrariesAsync(Models.Database.Host host, CancellationToken token)
    {
        var channel = channelManager.GetChannel(host);
        Libraries.LibrariesClient librariesClient = new(channel);
        var reply = await librariesClient.ListAsync(new(), cancellationToken: token);
        return reply.Labels;
    }

    public Task<Op> InstallAsync(Models.Database.Host host, string hostGameHostId, CancellationToken cancellationToken)
        => TriggerActionAsync(host, hostGameHostId, InstallAsync, cancellationToken);

    private static async Task<Op> InstallAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.InstallAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> UninstallAsync(Models.Database.Host host, string hostGameHostId, CancellationToken cancellationToken)
        => TriggerActionAsync(host, hostGameHostId, UninstallAsync, cancellationToken);

    private static async Task<Op> UninstallAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.UninstallAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> RepairAsync(Models.Database.Host host, string hostGameHostId, CancellationToken cancellationToken)
        => TriggerActionAsync(host, hostGameHostId, RepairAsync, cancellationToken);

    private static async Task<Op> RepairAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.RepairAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> StartAsync(Models.Database.Host host, string hostGameHostId, CancellationToken cancellationToken)
        => TriggerActionAsync(host, hostGameHostId, StartAsync, cancellationToken);

    private static async Task<Op> StartAsync(
        Games.GamesClient gamesClient, 
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.StartAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    public Task<Op> StopAsync(Models.Database.Host host, string hostGameHostId, CancellationToken cancellationToken)
        => TriggerActionAsync(host, hostGameHostId, StopAsync, cancellationToken);

    private static async Task<Op> StopAsync(
        Games.GamesClient gamesClient,
        GameIdentifier gameIdentifier,
        CancellationToken cancellationToken)
    {
        var result = await gamesClient.StopAsync(new() { GameIdentifier = gameIdentifier }, cancellationToken: cancellationToken);
        return result.Op;
    }

    private async Task<Op> TriggerActionAsync(
        Models.Database.Host host,
        string hostGameHostId,
        Func<Games.GamesClient, GameIdentifier, CancellationToken, Task<Op>> action,
        CancellationToken cancellationToken)
    {
        var channel = channelManager.GetChannel(host);
        Games.GamesClient gamesClient = new(channel);
        return await action(gamesClient, new() { Id = Guid.Parse(hostGameHostId).ToByteString() }, cancellationToken);
    }
}
