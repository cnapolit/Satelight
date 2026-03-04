using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class DatabaseService(ILogger<DatabaseService> logger) : Database.DatabaseBase
{
    public override Task<GetCacheReply> GetCache(GetCacheBody body, ServerCallContext context)
        => Pipe.SendRequestAsync(
            logger, RequestType.GetCache, body, GetCacheReply.Parser, context.CancellationToken);

    public override Task<UpdateDatabaseReply> Update(UpdateDatabaseBody request, ServerCallContext context) => base.Update(request, context);
}