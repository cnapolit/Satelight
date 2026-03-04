using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class OpsService(ILogger<OpsService> logger) : Ops.OpsBase
{
    public override Task<ListOpsReply> ListOps(ListOpsBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.ListOps, body, ListOpsReply.Parser, context.CancellationToken);

    public override Task<GetOpReply> GetOp(GetOpBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.GetOp, body, GetOpReply.Parser, context.CancellationToken);
}