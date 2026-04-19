using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

internal class GetCoverImageHandler(IPlayniteApi playniteApi)
    : GetImageHandler<GetGameCoverRequest, GetGameCoverResponse>(playniteApi)
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetGameCoverRequest request, CancellationToken token)
        => await GetImagesAsync(request, Plugin.Models.MediaFileType.DesktopCover, token);
}
