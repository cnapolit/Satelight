using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;

namespace Server.Services.Protos;

public class DatabaseService(ILogger<GamesService> logger, IDbContextFactory<DatabaseContext> databaseContextFactory) : Satelight.Protos.Core.Database.DatabaseBase
{
    public override Task<UpdateDatabaseReply> Update(UpdateDatabaseBody request, ServerCallContext context) => base.Update(request, context);

    public override async Task<GetCacheReply> GetCache(GetCacheBody request, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        GetCacheReply reply = new();
        var genresTask    = ProtoMapper.AddLabelsAsync(reply.Genres,             databaseContext.Genres,             context.CancellationToken);
        var statusTask    = ProtoMapper.AddLabelsAsync(reply.CompletionStatuses, databaseContext.CompletionStatuses, context.CancellationToken);
        var platformsTask = ProtoMapper.AddLabelsAsync(reply.Platforms,          databaseContext.Platforms,          context.CancellationToken);
        var tagsTask      = ProtoMapper.AddLabelsAsync(reply.Tags,               databaseContext.Tags,               context.CancellationToken);
        await Task.WhenAll(genresTask, statusTask, platformsTask, tagsTask);
        return reply;
    }
}
