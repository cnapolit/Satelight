using Common.Utility.Functions;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;
using Plugin.Common.Extensions;
using Plugin.Models;
using System.IO;

namespace HostPlugin.Services.RequestHandlers;

internal abstract class GetImageHandler<TReq, TResp>(IPlayniteApi playniteApi) : ImageHandler<TReq>
    where TReq  : GameRequest
    where TResp : ListFilesResponse, new()
{
    private static readonly string LibraryFilesPath = Path.Combine("Library", "Files");

    protected async ValueTask<TResp> GetImagesAsync(TReq request, MediaFileType mediaFileType, CancellationToken token)
    {
        TResp response = new();
        var playniteSubPathImage = playniteApi.Library.Games
            .Get(request.Id)
           ?.MediaFiles
           ?.FirstOrDefault(f => f.GetMediaFileType() == mediaFileType)
           ?.Path;

        if (Doc.TryGet(out var playniteImage, playniteApi.AppInfo.ConfigurationDirectory, LibraryFilesPath, playniteSubPathImage))
        {
            response.Files.Add(playniteImage);
        }

        return response;
    }
}
