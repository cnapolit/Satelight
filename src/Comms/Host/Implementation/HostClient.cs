using Comms.Common.Interface.Models;
using Comms.Host.Interface;
using Comms.Host.Interface.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Comms.Host.Implementation.Services.RequestProxy;
using GetOpRequest = Comms.Common.Interface.Models.GetOpRequest;
using GetOpsRequest = Comms.Common.Interface.Models.GetOpsRequest;

namespace Comms.Host.Implementation;

public class HostClient(string ipAddress, int port) : IHostClient
{
    public IAsyncEnumerable<StreamGamesResponse> StreamGamesAsync(StreamGamesRequest request, CancellationToken token)
        => CreateProxy<StreamGamesProxy>().StreamGamesAsync(request, token);

    public Task<GetCacheResponse> GetCacheAsync(GetCacheRequest getCacheRequest, CancellationToken token) 
        => CreateProxy<GetCacheProxy>().SendAsync(getCacheRequest, token);

    public Task<GetTagsResponse> GetTagsAsync(CancellationToken token)
        => throw new NotImplementedException();

    public Task<GetPlatformsResponse> GetPlatformsAsync(CancellationToken token)
        => throw new NotImplementedException();

    public Task<GetGenresResponse> GetGenresAsync(CancellationToken token)
        => throw new NotImplementedException();

    public Task<GetSourcesResponse> GetSourcesAsync(CancellationToken token)
        => throw new NotImplementedException();

    public Task<GetOpResponse> GetOpAsync(GetOpRequest getOpRequest, CancellationToken token)
    => CreateProxy<GetOpProxy>().SendAsync(getOpRequest, token);

    public Task<GetOpsResponse> GetOpsAsync(GetOpsRequest request, CancellationToken token)
        => CreateProxy<GetOpsProxy>().SendAsync(request, token);

    public Task<GetGameResponse> GetGameAsync(GetGameRequest getGameRequest, CancellationToken token)
        => throw new NotImplementedException();

    public Task<StartGameResponse> StartGameAsync(StartGameRequest request, CancellationToken token)
        => CreateProxy<StartGameProxy>().SendAsync(request, token);

    public Task<StopGameResponse> StopGameAsync(StopGameRequest request, CancellationToken token)
        => CreateProxy<StopGameProxy>().SendAsync(request, token);

    public Task<UpdateGameResponse> UpdateGameAsync(UpdateGameRequest request, CancellationToken token)
        => CreateProxy<UpdateGameProxy>().SendAsync(request, token);

    public Task<RepairGameResponse> RepairGameAsync(RepairGameRequest request, CancellationToken token)
        => CreateProxy<RepairGameProxy>().SendAsync(request, token);

    public Task<MoveGameResponse> MoveGameAsync(MoveGameRequest request, CancellationToken token)
        => throw new NotImplementedException();

    public Task<InstallGameResponse> InstallGameAsync(InstallGameRequest request, CancellationToken token)
        => CreateProxy<InstallGameProxy>().SendAsync(request, token);

    public Task<UninstallGameResponse> UninstallGameAsync(UninstallGameRequest request, CancellationToken token)
        => CreateProxy<UninstallGameProxy>().SendAsync(request, token);

    public Task<RemoveGameResponse> RemoveGameAsync(RemoveGameRequest request, CancellationToken token)
        => CreateProxy<RemoveGameProxy>().SendAsync(request, token);

    private TProxy CreateProxy<TProxy>() where TProxy : RequestProxy, new()
        => new() { IpAddress = ipAddress, Port = port };
}
