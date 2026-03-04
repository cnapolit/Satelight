using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetCacheHandler
{
    public GetCacheResponse Handle(DatabaseService databaseService) => new()
    {
        Filters = databaseService.GetFilters(),
        Genres = databaseService.GetGenres(),
        Platforms = databaseService.GetPlatforms(),
        CompletionStatuses = databaseService.GetCompletionStatuses(),
        Tags = databaseService.GetTags()
    };
}