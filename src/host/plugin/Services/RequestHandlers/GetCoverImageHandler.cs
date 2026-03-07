using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

internal class GetCoverImageHandler(IPlayniteAPI playniteApi)
    : GetImageHandler<GetGameCoverRequest, GetGameCoverResponse>(playniteApi)
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetGameCoverRequest request, CancellationToken token)
        => await GetImagesAsync(request, true, token);
}
