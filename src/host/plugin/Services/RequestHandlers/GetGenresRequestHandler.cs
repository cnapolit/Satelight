using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetGenresRequestHandler
    : GetLabelsRequestHandler<GetGenresRequest, GetGenresResponse>, IRequestHandler<GetGenresRequest>
{
    protected override Label[] GetLabels() => DatabaseService.GetGenres();
}
