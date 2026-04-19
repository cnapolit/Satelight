using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateCoverHandler(IPlayniteApi playniteApi)
    : UpdateImageHandler<UpdateCoverRequest, UpdateCoverResponse>(playniteApi)
{
    public override async ValueTask<SatelightResponse> HandleAsync(UpdateCoverRequest request, CancellationToken token)
        => await UpdateGameImagesAsync(request, Plugin.Models.MediaFileType.DesktopCover, token);
}
