using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class PlatformsService(ILogger<PlatformsService> logger) : Platforms.PlatformsBase
{
    public override Task<ListPlatformsReply> List(ListPlatformsBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<ListPlatformsReply>(
            logger, RequestType.ListPlatforms, request, ListPlatformsReply.Parser, context.CancellationToken);
}