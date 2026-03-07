using Comms.Common.Interface.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class GetCacheHandler(IDatabaseService databaseService) : RequestHandler<GetCacheRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetCacheRequest getCacheRequest, CancellationToken _) => new GetCacheResponse
    {
        Filters = databaseService.GetFilters(),
        Genres = databaseService.GetGenres(),
        Platforms = databaseService.GetPlatforms(),
        CompletionStatuses = databaseService.GetCompletionStatuses(),
        Tags = databaseService.GetTags()
    };
}