using Comms.Common.Interface.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class GetFiltersHandler(IDatabaseService databaseService) : RequestHandler<GetFiltersRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetFiltersRequest getFiltersRequest, CancellationToken _) => new GetFiltersResponse
    {
        Filters = databaseService.GetFilters()
    };
}