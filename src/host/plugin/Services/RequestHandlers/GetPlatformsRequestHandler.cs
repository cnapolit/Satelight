using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetPlatformsRequestHandler
    : GetLabelsRequestHandler<GetPlatformsRequest, GetPlatformsResponse>, IRequestHandler<GetPlatformsRequest>
{
    protected override Label[] GetLabels() => DatabaseService.GetPlatforms();
}
