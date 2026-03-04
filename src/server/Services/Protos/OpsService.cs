using Common.Utility.Extensions;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Satelight.Protos.Core;
using Server.Database;

namespace Server.Services.Protos;

public class OpsService(ILogger<GamesService> logger, IDbContextFactory<DatabaseContext> databaseContextFactory) : Ops.OpsBase
{
    public override async Task<GetOpReply> GetOp(GetOpBody body, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        var opId = body.Id.ToGuid();
        var op = await databaseContext.Operations.FindAsync(opId, context.CancellationToken)
              ?? throw new RpcException(new(StatusCode.NotFound, $"Operation with Id '{opId}' does not exist"));

        return new() { Info = ProtoMapper.CreateOpInfo(op) };
    }

    public override async Task<ListOpsReply> ListOps(ListOpsBody body, ServerCallContext context)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(context.CancellationToken);
        var ops = await databaseContext.Operations.AsNoTracking().ToListAsync(context.CancellationToken);
        ListOpsReply reply = new();
        reply.Ops.AddRange(ops.Select(ProtoMapper.CreateOp));
        return reply;
    }
}
