using Grpc.Core;
using Piped;
using Satelight.Protos.Core;
using Service.Common;

namespace Service.Services;

public class TagsService(ILogger<TagsService> logger) : Tags.TagsBase
{
    public override Task<ListTagsReply> List(ListTagsBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<ListTagsReply>(
            logger, RequestType.ListTags, request, ListTagsReply.Parser, context.CancellationToken);
}