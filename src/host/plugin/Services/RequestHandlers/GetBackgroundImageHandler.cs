using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

internal class GetBackgroundImageHandler(IPlayniteAPI playniteApi)
    : GetImageHandler<GetGameBackgroundRequest, GetGameBackgroundResponse>(playniteApi), IGetBackgroundImageHandler
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetGameBackgroundRequest request, CancellationToken token)
        => await GetImagesAsync(request, false, token);
}
