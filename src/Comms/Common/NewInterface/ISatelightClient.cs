using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface.Models;

namespace Comms.Common.Interface;

public interface ISatelightClient
{
    IAsyncEnumerable<StreamGamesResponse> StreamGamesAsync(StreamGamesRequest request, CancellationToken token);
    Task<GetCacheResponse>                GetCacheAsync(GetCacheRequest getCacheRequest, CancellationToken token);
    Task<GetTagsResponse>                 GetTagsAsync(CancellationToken token);
    Task<GetPlatformsResponse>            GetPlatformsAsync(CancellationToken token);
    Task<GetGenresResponse>               GetGenresAsync(CancellationToken token);
    Task<GetSourcesResponse>              GetSourcesAsync(CancellationToken token);
    Task<GetOpResponse>                   GetOpAsync(GetOpRequest getOpRequest, CancellationToken token);
    Task<GetOpsResponse>                  GetOpsAsync(GetOpsRequest request, CancellationToken token);
    Task<GetGameResponse>                 GetGameAsync(GetGameRequest getGameRequest, CancellationToken token);
}