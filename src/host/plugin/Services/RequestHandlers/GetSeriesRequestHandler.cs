using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetSeriesRequestHandler
    : GetLabelsRequestHandler<GetSeriesRequest, GetSeriesResponse>, IRequestHandler<GetSeriesRequest>
{
    protected override Label[] GetLabels() => DatabaseService.GetSeries();
}
