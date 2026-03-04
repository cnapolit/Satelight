using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetFiltersHandler
{
    public GetFiltersResponse Handle(DatabaseService databaseService) => new()
    {
        Filters = databaseService.GetFilters()
    };
}