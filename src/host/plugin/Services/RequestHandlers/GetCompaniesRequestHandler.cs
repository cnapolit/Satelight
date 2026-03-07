using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetCompaniesRequestHandler
    : GetLabelsRequestHandler<GetCompaniesRequest, GetCompaniesResponse>, IRequestHandler<GetCompaniesRequest>
{
    protected override Label[] GetLabels() => DatabaseService.GetCompanies();
}
