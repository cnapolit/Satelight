using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateCoverHandler(IPlayniteAPI playniteApi) : UpdateImageHandler<UpdateCoverRequest, UpdateCoverResponse>(playniteApi)
{
    public override ValueTask<UpdateCoverResponse> HandleAsync(UpdateCoverRequest request, CancellationToken token)
        => UpdateGameImagesAsync<UpdateCoverResponse>(request, true, token);
}
