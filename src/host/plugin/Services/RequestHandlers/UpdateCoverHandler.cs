using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateCoverHandler(IPlayniteAPI playniteApi)
    : UpdateImageHandler<UpdateCoverRequest, UpdateCoverResponse>(playniteApi)
{
    public override async ValueTask<SatelightResponse> HandleAsync(UpdateCoverRequest request, CancellationToken token)
        => await UpdateGameImagesAsync(request, true, token);
}
