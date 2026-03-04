using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class FeaturesService(ILogger<FeaturesService> logger) : Features.FeaturesBase
{
    public override Task<ListFeaturesReply> List(ListFeaturesBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<ListFeaturesReply>(
            logger, RequestType.ListFeatures, request, ListFeaturesReply.Parser, context.CancellationToken);
}