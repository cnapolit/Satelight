using Common.Utility.Extensions;
using Common.Utility.Functions;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;
using Plugin.Models;
using System.IO;
using System.Text.Json.Nodes;

namespace HostPlugin.Services.RequestHandlers;

public abstract class UpdateImageHandler<TReq, TRep>(IPlayniteApi playniteApi) : ImageHandler<TReq>
    where TReq : SatelightRequest
    where TRep : SuccessResponse, new()
{
    protected async ValueTask<TRep> UpdateGameImagesAsync(
        UpdateGameFileRequest request, MediaFileType mediaFileType, CancellationToken token)
    {
        // TODO: figure out P11 url support
        throw new NotImplementedException();
        TRep resp = new();
        var game = playniteApi.Library.Games.Get(request.Id);
        if (game is null)
        {
            return resp;
        }

        var coverImageName = Path.GetFileName(request.NewPath);
        //if (coverImageName == request.FileName)
        //{
        //    if (isCover)
        //    {
        //        game.CoverImage = request.NewPath;
        //    }
        //    else
        //    {
        //        game.BackgroundImage = request.NewPath;
        //    }
        //    playniteApi.Library.Games.Update(game);
        //    resp.Success = true;
        //    return resp;
        //}


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