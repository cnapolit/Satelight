using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateBackgroundHandler(IPlayniteApi playniteApi)
    : UpdateImageHandler<UpdateBackgroundRequest, UpdateBackgroundResponse>(playniteApi), IRequestHandler<UpdateBackgroundRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(UpdateBackgroundRequest request, CancellationToken token)
        => await UpdateGameImagesAsync(request, Plugin.Models.MediaFileType.DesktopBackground, token);
}
