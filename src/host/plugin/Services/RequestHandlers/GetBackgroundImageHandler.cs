using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

internal class GetBackgroundImageHandler(IPlayniteApi playniteApi)
    : GetImageHandler<GetGameBackgroundRequest, GetGameBackgroundResponse>(playniteApi)
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetGameBackgroundRequest request, CancellationToken token)
        => await GetImagesAsync(request, Plugin.Models.MediaFileType.DesktopBackground, token);
}
