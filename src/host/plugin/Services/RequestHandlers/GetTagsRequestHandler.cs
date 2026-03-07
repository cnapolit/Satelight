using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetTagsRequestHandler
    : GetLabelsRequestHandler<GetTagsRequest, GetTagsResponse>, IRequestHandler<GetTagsRequest>
{
    protected override Label[] GetLabels() => DatabaseService.GetTags();
}
