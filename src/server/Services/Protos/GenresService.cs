using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;

namespace Server.Services.Protos;

public class GenresService(ILogger<GamesService> logger, IDbContextFactory<DatabaseContext> databaseContextFactory) : Genres.GenresBase
{
    public override async Task<ListGenresReply> List(ListGenresBody request, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        ListGenresReply reply = new();
        await ProtoMapper.AddLabelsAsync(reply.Labels, databaseContext.Genres, context.CancellationToken);
        return reply;
    }
}