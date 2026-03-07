using Common.Utility.Extensions;
using Common.Utility.Functions;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public abstract class UpdateImageHandler<TReq, TRep>(IPlayniteAPI playniteApi)
    : ImageHandler, IRequestHandler<TReq, TRep>
{
    public abstract ValueTask<TRep> HandleAsync(TReq request, CancellationToken token);

    protected async ValueTask<TResp> UpdateGameImagesAsync<TResp>(
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
                CommentHandling = JsonCommentHandling.Skip,
                MaxDepth = 4,
                AllowTrailingCommas = true
            }, token);

            if (json is null)
            {
                return resp;
            }

            var updated = false;
            foreach (var jsonEntry in GetBackgroundChangerImages(json, imagesDir, isCover))
                if (Path.GetFileName(jsonEntry.Item2) == request.FileName)
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
}