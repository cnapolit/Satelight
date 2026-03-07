using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetFeaturesRequestHandler
    : GetLabelsRequestHandler<GetFeaturesRequest, GetFeaturesResponse>, IRequestHandler<GetFeaturesRequest>
{
    protected override Label[] GetLabels() => DatabaseService.GetFeatures();
}
