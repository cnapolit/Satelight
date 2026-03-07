using Common.Utility.Functions;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using Service.Common.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

internal abstract class GetImageHandler<TReq, TResp>(IPlayniteAPI playniteApi) : ImageHandler, IRequestHandler<TReq, TResp>
    where TReq  : GameRequest
    where TResp : ListFilesResponse, new()
{
    private static readonly string LibraryFilesPath = Path.Combine("Library", "Files");

    public abstract ValueTask<TResp> HandleAsync(TReq request, CancellationToken token);

    protected async ValueTask<TResp> GetImagesAsync(TReq request, bool isCover, CancellationToken token)
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
