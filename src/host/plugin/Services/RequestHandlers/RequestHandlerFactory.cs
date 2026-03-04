using System;
using System.Collections.Generic;
using Common.Utility.Extensions;
using Comms.Common.Interface;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Common.Utility.Functions;
using Service.Common.Extensions;

namespace HostPlugin.Services.RequestHandlers;

public class RequestHandlerFactory(
    IPlayniteAPI playniteApi, ActionTracker actionTracker, GetGamesService getGamesService, DatabaseService databaseService)
{
    private static readonly string BackgroundChangerJsonPath;
    private static readonly string BackgroundChangerImagePath;
    private static readonly string LibraryFilesPath = Path.Combine("Library", "Files");

    static RequestHandlerFactory()
    {
        var backgroundChangerSubPath = Path.Combine("ExtensionsData", "3afdd02b-db6c-4b60-8faa-2971d6dfad2a");
        BackgroundChangerJsonPath  = Path.Combine(backgroundChangerSubPath, "BackgroundChanger");
        BackgroundChangerImagePath = Path.Combine(backgroundChangerSubPath, "Images");
    }

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
            GetGameRequest getGameRequest => new GetGameResponse { Game = getGamesService.GetGame(getGameRequest) },
            GetGameCoverRequest getGameCoverRequest => await GetImagesAsync<GetGameCoverResponse>(getGameCoverRequest, true, token),
            GetGameBackgroundRequest getGameBackgroundRequest => await GetImagesAsync<GetGameBackgroundResponse>(getGameBackgroundRequest, false, token),
            InitializeRequest                         => new InitializeResponse { Path = playniteApi.Paths.ConfigurationPath, Port = 5156 },
            StartGameRequest         startGameRequest => new StartGameHandler(playniteApi, actionTracker, getGamesService).Handle(startGameRequest),
            InstallGameRequest     installGameRequest => new InstallGameHandler(playniteApi, actionTracker, getGamesService).Handle(installGameRequest),
            UninstallGameRequest uninstallGameRequest => new UninstallGameHandler(playniteApi, actionTracker, getGamesService).Handle(uninstallGameRequest),
            RemoveGameRequest       removeGameRequest => new RemoveGameHandler().Handle(playniteApi, getGamesService, removeGameRequest),
            GetTagsRequest                            => new GetLabelsRequestHandler<GetTagsResponse>(databaseService.GetTags).Handle(),
            GetGenresRequest                          => new GetLabelsRequestHandler<GetGenresResponse>(databaseService.GetGenres).Handle(),
            GetLibrariesRequest                         => new GetLibrariesResponse { Items = playniteApi.Addons.Plugins.As<LibraryPlugin>().Select(p => new Label { Id = p.Id, Name = p.Name }).ToArray() } ,
            GetPlatformsRequest                       => new GetLabelsRequestHandler<GetPlatformsResponse>(databaseService.GetPlatforms).Handle(),
            GetFeaturesRequest => new GetLabelsRequestHandler<GetFeaturesResponse>(databaseService.GetFeatures).Handle(),
            GetSeriesRequest => new GetLabelsRequestHandler<GetSeriesResponse>(databaseService.GetSeries).Handle(),
            GetCompaniesRequest => new GetLabelsRequestHandler<GetCompaniesResponse>(databaseService.GetCompanies).Handle(),
            GetOpRequest                 getOpRequest => new GetOpHandler(actionTracker).Handle(getOpRequest),
            GetOpsRequest                             => new GetOpsHandler(actionTracker).Handle(),
            GetCacheRequest                           => new GetCacheHandler().Handle(databaseService),
            GetFiltersRequest                         => new GetFiltersHandler().Handle(databaseService),
            CountGamesRequest => new CountGamesResponse { Count = playniteApi.Database.GetFilteredGames(new() { Hidden = false }).Count() },
            UpdateCoverRequest updateCoverRequest => await UpdateGameImagesAsync<UpdateCoverResponse>(updateCoverRequest, true, token),
            UpdateBackgroundRequest updateBackgroundRequest => await UpdateGameImagesAsync<UpdateBackgroundResponse>(updateBackgroundRequest, false, token),
            //UpdateGameRequest updateGameRequest => new UpdateGameRequestHandler(playniteApi, getGamesService).Handle(playniteApi, updateGameRequest),
            //RepairGameRequest repairGameRequest => new RepairGameHandler(playniteApi, actionTracker, getGamesService).Handle(playniteApi, repairGameRequest),
            //MoveGameRequest moveGameRequest => new MoveGameHandler(playniteApi, actionTracker, getGamesService).Handle(playniteApi, moveGameRequest),
            _ => throw new($"Unsupported request '{request.GetType()}'")
        };
        await connection.SendResponseAsync(response, token);
    }

    private async ValueTask<TResp> GetImagesAsync<TResp>(GameRequest request, bool isCover, CancellationToken token)
        where TResp : ListFilesResponse, new()
    {
        TResp response = new();
        var game = playniteApi.Database.Games.Get(request.Id);
        if (game is null)
        {
            return response;
        }

        var playniteSubPathImage = isCover ? game.CoverImage : game.BackgroundImage;
        if (Doc.TryGet(out var playniteImage, playniteApi.Paths.ConfigurationPath, LibraryFilesPath, playniteSubPathImage))
        {
            response.Files.Add(playniteImage);
        }
        
        //var gameIdStr = game.Id.ToString();
        //if      (Dir.TryGet(out var imagesDir, playniteApi.Paths.ConfigurationPath, BackgroundChangerImagePath, gameIdStr))
        //using   (var json = await GetAsync(gameIdStr, token))
        //foreach (var image in GetBackgroundChangerImages(json, imagesDir, isCover))
        //{
        //    response.Files.Add(image.Item2);
        //}

        return response;
    }

    private async ValueTask<TResp> UpdateGameImagesAsync<TResp>(
        UpdateGameFileRequest request, bool isCover, CancellationToken token) where TResp : SuccessResponse, new()
    {
        TResp resp = new();
        var game = playniteApi.Database.Games.Get(request.Id);
        if (game is null)
        {
            return resp;
        }

        var coverImageName = Path.GetFileName(request.NewPath);
        if (coverImageName == request.FileName)
        {
            if (isCover)
            {
                game.CoverImage = request.NewPath;
            }
            else
            {
                game.BackgroundImage = request.NewPath;
            }
            playniteApi.Database.Games.Update(game);
            resp.Success = true;
            return resp;
        }

        var gameIdStr = game.Id.ToString();
        if (Dir.TryGet(out var imagesDir, playniteApi.Paths.ConfigurationPath, BackgroundChangerImagePath, gameIdStr))
        {
            if (!Doc.TryGet(out var jsonPath, playniteApi.Paths.ConfigurationPath, BackgroundChangerJsonPath, $"{gameIdStr}.json"))
            {
                return resp;
            }

            using var fileStream = File.Open(jsonPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var json = await JsonNode.ParseAsync(fileStream, new() { PropertyNameCaseInsensitive = true }, new()
            {
                CommentHandling     = JsonCommentHandling.Skip,
                MaxDepth            = 4,
                AllowTrailingCommas = true
            }, token);

            if (json is null)
            {
                return resp;
            }

            var updated = false;
            foreach (var jsonEntry in GetBackgroundChangerImages(json, imagesDir, isCover))
            if      (Path.GetFileName(jsonEntry.Item2) == request.FileName)
            {
                jsonEntry.Item1["Name"] = request.NewPath;
                updated = true;
                break;
            }

            if (updated)
            {
                await using var writer = new Utf8JsonWriter(fileStream, new() { Indented = true });
                json.WriteTo(writer);
            }
        }

        return resp;
    }

    private static IEnumerable<Tuple<JsonElement, string>> GetBackgroundChangerImages(JsonDocument? json, string imagesDir, bool isCover)
    {
        if           (json?.RootElement.ValueKind is JsonValueKind.Object)
        foreach      (var item in json.RootElement.AsArr("Items"))
        if           (item.ValueKind is JsonValueKind.Object)
        if           (item.IsFalse  ("IsDefault"))
        if           (item.IsTrue   ("IsCover") == isCover)
        if           (item.TryGetStr("Name", out var name)) 
        if           (Doc.TryGet(out var filePath, imagesDir, name))
        yield return new(item, filePath);
    }

    private static IEnumerable<Tuple<JsonObject, string>> GetBackgroundChangerImages(JsonNode? json, string imagesDir, bool isCover)
    {
        if           (json is JsonObject jsonObj)
        if           (jsonObj.TryGet("Items", out JsonArray items))
        foreach      (var item in items.As<JsonObject>())
        if           (item.IsFalse("IsDefault"))
        if           (item.IsTrue   ("IsCover") == isCover)
        if           (item.TryGet("Name", out string name)) 
        if           (Doc.TryGet(out var filePath, imagesDir, name))
        yield return new(item, filePath);
    }

    private async Task<JsonDocument?> GetAsync(string gameIdStr, CancellationToken token)
    {
        if           (Doc.TryGet(out var jsonPath, playniteApi.Paths.ConfigurationPath, BackgroundChangerJsonPath, $"{gameIdStr}.json"))
        using        (var fileStream = File.Open(jsonPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
        return await JsonDocument.ParseAsync(fileStream, new()
        {
            AllowTrailingCommas = true,
            CommentHandling     = JsonCommentHandling.Skip,
            MaxDepth            = 4
        }, token);

        return null;
    }
}