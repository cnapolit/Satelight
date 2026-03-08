using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public interface IDatabaseService
{
    Filter[] GetFilters();
    Label[] GetTags();
    Label[] GetGenres();
    Label[] GetPlatforms();
    Label[] GetCompletionStatuses();
    Label[] GetSources();
    Label[] GetFeatures();
    Label[] GetCompanies();
    Label[] GetSeries();
}
