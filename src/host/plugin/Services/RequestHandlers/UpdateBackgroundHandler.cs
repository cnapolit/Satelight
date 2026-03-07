using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Playnite.SDK;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateBackgroundHandler(IPlayniteAPI playniteApi)
    : UpdateImageHandler<UpdateBackgroundRequest, UpdateBackgroundResponse>(playniteApi), IRequestHandler<UpdateBackgroundRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(UpdateBackgroundRequest request, CancellationToken token)
        => await UpdateGameImagesAsync(request, false, token);
}
