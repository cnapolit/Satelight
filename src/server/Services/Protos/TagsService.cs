using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;

namespace Server.Services.Protos;

public class TagsService(ILogger<GamesService> logger, IDbContextFactory<DatabaseContext> databaseContextFactory) : Tags.TagsBase
{
    public override async Task<ListTagsReply> List(ListTagsBody request, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        ListTagsReply reply = new();
        await ProtoMapper.AddLabelsAsync(reply.Labels, databaseContext.Tags, context.CancellationToken);
        return reply;
    }
}