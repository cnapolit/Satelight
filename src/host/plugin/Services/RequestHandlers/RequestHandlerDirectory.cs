using Comms.Common.Interface;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class RequestHandlerDirectory(
    IStopGameRequestHandler stopGameRequestHandler,
    IGamesStreamHandler gamesStreamHandler,
    IRequestHandler<GetGameRequest> getGameRequestHandler,
    IRequestHandler<GetGameCoverRequest> getGameCoverRequestHandler,
    IRequestHandler<GetGameBackgroundRequest> getGameBackgroundRequestHandler,
    IRequestHandler<InitializeRequest> initializeRequestHandler,
    IRequestHandler<StartGameRequest> startGameRequestHandler,
    IRequestHandler<InstallGameRequest> installGameRequestHandler,
    IRequestHandler<UninstallGameRequest> uninstallGameRequestHandler,
    IRequestHandler<RemoveGameRequest> removeGameRequestHandler,
    IRequestHandler<GetTagsRequest> getTagsRequestHandler,
    IRequestHandler<GetGenresRequest> getGenresRequestHandler,
    IRequestHandler<GetLibrariesRequest> getLibrariesRequestHandler,
    IRequestHandler<GetPlatformsRequest> getPlatformsRequestHandler,
    IRequestHandler<GetFeaturesRequest> getFeaturesRequestHandler,
    IRequestHandler<GetSeriesRequest> getSeriesRequestHandler,
    IRequestHandler<GetCompaniesRequest> getCompaniesRequestHandler,
    IRequestHandler<GetOpRequest> getOpRequestHandler,
    IRequestHandler<GetOpsRequest> getOpsRequestHandler,
    IRequestHandler<GetCacheRequest> getCacheRequestHandler,
    IRequestHandler<GetFiltersRequest> getFiltersRequestHandler,
    IRequestHandler<CountGamesRequest> countGamesRequestHandler,
    IRequestHandler<UpdateCoverRequest> updateCoverRequestHandler,
    IRequestHandler<UpdateBackgroundRequest> updateBackgroundRequestHandler) : IRequestHandlerDirectory
{
    public async Task HandleAsync(ISatelightConnection connection, CancellationToken token)
    {
        var request = await connection.ReadRequestAsync(token);
        if (request is StopGameRequest stopGameRequest)
        {
            await stopGameRequestHandler.HandleRequestAsync(stopGameRequest, connection, token);
            return;
        }

        if (request is StreamGamesRequest streamGamesRequest)
        {
            await gamesStreamHandler.HandleRequestAsync(streamGamesRequest, connection, token);
            return;
        }

        IRequestHandler handler = request switch
        {
            GetGameRequest => getGameRequestHandler,
            GetGameCoverRequest => getGameCoverRequestHandler,
            GetGameBackgroundRequest => getGameBackgroundRequestHandler,
            InitializeRequest => initializeRequestHandler,
            StartGameRequest => startGameRequestHandler,
            InstallGameRequest => installGameRequestHandler,
            UninstallGameRequest => uninstallGameRequestHandler,
            RemoveGameRequest => removeGameRequestHandler,
            GetTagsRequest => getTagsRequestHandler,
            GetGenresRequest => getGenresRequestHandler,
            GetLibrariesRequest => getLibrariesRequestHandler,
            GetPlatformsRequest => getPlatformsRequestHandler,
            GetFeaturesRequest => getFeaturesRequestHandler,
            GetSeriesRequest => getSeriesRequestHandler,
            GetCompaniesRequest => getCompaniesRequestHandler,
            GetOpRequest => getOpRequestHandler,
            GetOpsRequest => getOpsRequestHandler,
            GetCacheRequest => getCacheRequestHandler,
            GetFiltersRequest => getFiltersRequestHandler,
            CountGamesRequest => countGamesRequestHandler,
            UpdateCoverRequest => updateCoverRequestHandler,
            UpdateBackgroundRequest => updateBackgroundRequestHandler,
            _ => throw new($"Unsupported request '{request.GetType()}'")
        };
        var response = await handler.HandleAsync(request, token);
        await connection.SendResponseAsync(response, token);
    }
}