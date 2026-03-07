using Comms.Common.Interface;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class RequestHandlerFactory(
    IPlayniteAPI playniteApi, ActionTracker actionTracker, GetGamesService getGamesService, DatabaseService databaseService)
{
    public async Task HandleAsync(ISatelightConnection connection, CancellationToken token)
    {
        var request = await connection.ReadRequestAsync(token);
        if (request is StopGameRequest stopGameRequest)
        {
            await new StopGameRequestHandler(actionTracker, connection).HandleRequestAsync(stopGameRequest, getGamesService, token);
            return;
        }

        if (request is StreamGamesRequest streamGamesRequest)
        {
            await new GamesStreamHandler(getGamesService).HandleRequestAsync(streamGamesRequest, connection, token);
            return;
        }

        SatelightResponse response = request switch
        {
            GetGameRequest getGameRequest => await new GetGameRequestHandler(getGamesService).HandleAsync(getGameRequest, token),
            GetGameCoverRequest getGameCoverRequest => await new GetCoverImageHandler(playniteApi).HandleAsync(getGameCoverRequest, token),
            GetGameBackgroundRequest getGameBackgroundRequest => await new GetBackgroundImageHandler(playniteApi).HandleAsync(getGameBackgroundRequest, token),
            InitializeRequest         initializeRequest => await new InitResponseHandler(playniteApi).HandleAsync(initializeRequest, token),
            StartGameRequest         startGameRequest => new StartGameHandler(playniteApi, actionTracker, getGamesService).Handle(startGameRequest),
            InstallGameRequest     installGameRequest => new InstallGameHandler(playniteApi, actionTracker, getGamesService).Handle(installGameRequest),
            UninstallGameRequest uninstallGameRequest => new UninstallGameHandler(playniteApi, actionTracker, getGamesService).Handle(uninstallGameRequest),
            RemoveGameRequest       removeGameRequest => new RemoveGameHandler().Handle(playniteApi, getGamesService, removeGameRequest),
            GetTagsRequest                            => new GetLabelsRequestHandler<GetTagsResponse>(databaseService.GetTags).Handle(),
            GetGenresRequest                          => new GetLabelsRequestHandler<GetGenresResponse>(databaseService.GetGenres).Handle(),
            GetLibrariesRequest getLibrariesRequest => await new GetLibrariesHandler(playniteApi).HandleAsync(getLibrariesRequest, token),
            GetPlatformsRequest                       => new GetLabelsRequestHandler<GetPlatformsResponse>(databaseService.GetPlatforms).Handle(),
            GetFeaturesRequest => new GetLabelsRequestHandler<GetFeaturesResponse>(databaseService.GetFeatures).Handle(),
            GetSeriesRequest => new GetLabelsRequestHandler<GetSeriesResponse>(databaseService.GetSeries).Handle(),
            GetCompaniesRequest => new GetLabelsRequestHandler<GetCompaniesResponse>(databaseService.GetCompanies).Handle(),
            GetOpRequest                 getOpRequest => new GetOpHandler(actionTracker).Handle(getOpRequest),
            GetOpsRequest                             => new GetOpsHandler(actionTracker).Handle(),
            GetCacheRequest                           => new GetCacheHandler().Handle(databaseService),
            GetFiltersRequest                         => new GetFiltersHandler().Handle(databaseService),
            CountGamesRequest countGamesRequest => await new CountGamesHandler(playniteApi).HandleAsync(countGamesRequest, token),
            UpdateCoverRequest updateCoverRequest =>await new UpdateCoverHandler(playniteApi).HandleAsync(updateCoverRequest, token),
            UpdateBackgroundRequest updateBackgroundRequest => await new UpdateBackgroundHandler(playniteApi).HandleAsync(updateBackgroundRequest, token),
            //UpdateGameRequest updateGameRequest => new UpdateGameRequestHandler(playniteApi, getGamesService).Handle(playniteApi, updateGameRequest),
            //RepairGameRequest repairGameRequest => new RepairGameHandler(playniteApi, actionTracker, getGamesService).Handle(playniteApi, repairGameRequest),
            //MoveGameRequest moveGameRequest => new MoveGameHandler(playniteApi, actionTracker, getGamesService).Handle(playniteApi, moveGameRequest),
            _ => throw new($"Unsupported request '{request.GetType()}'")
        };
        await connection.SendResponseAsync(response, token);
    }
}