using Common.Utility.Extensions;
using Comms.Common.Interface.Models;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services.RequestHandlers;

public class GetLibrariesHandler(IPlayniteAPI playniteApi) : IRequestHandler<GetLibrariesRequest, GetLibrariesResponse>
{
    public async ValueTask<GetLibrariesResponse> HandleAsync(GetLibrariesRequest request, CancellationToken token)
        => new() { Items = playniteApi.Addons.Plugins.As<LibraryPlugin>().Select(p => new Label { Id = p.Id, Name = p.Name }).ToArray() };

}
