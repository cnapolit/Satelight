using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateBackgroundHandler(IPlayniteAPI playniteApi) : UpdateImageHandler<UpdateBackgroundRequest, UpdateBackgroundResponse>(playniteApi)
{
    public override ValueTask<UpdateBackgroundResponse> HandleAsync(UpdateBackgroundRequest request, CancellationToken token)
        => UpdateGameImagesAsync<UpdateBackgroundResponse>(request, false, token);
}
