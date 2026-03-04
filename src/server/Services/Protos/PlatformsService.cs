using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;

namespace Server.Services.Protos;

public class PlatformsService(IDbContextFactory<DatabaseContext> databaseContextFactory) : Platforms.PlatformsBase
{
    public override async Task<ListPlatformsReply> List(ListPlatformsBody request, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        ListPlatformsReply reply = new();
        await ProtoMapper.AddLabelsAsync(reply.Labels, databaseContext.Platforms, context.CancellationToken);
        return reply;
    }
}