using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

internal class GetBackgroundImageHandler(IPlayniteAPI playniteApi)
    : GetImageHandler<GetGameBackgroundRequest, GetGameBackgroundResponse>(playniteApi)
{
    public override ValueTask<GetGameBackgroundResponse> HandleAsync(GetGameBackgroundRequest request, CancellationToken token)
        => GetImagesAsync(request, false, token);
}
