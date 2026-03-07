using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

internal class GetCoverImageHandler(IPlayniteAPI playniteApi)
    : GetImageHandler<GetGameCoverRequest, GetGameCoverResponse>(playniteApi)
{
    public override ValueTask<GetGameCoverResponse> HandleAsync(GetGameCoverRequest request, CancellationToken token)
        => GetImagesAsync(request, true, token);
}
