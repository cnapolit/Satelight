using Comms.Common.Interface.Models;
using Playnite;

namespace HostPlugin.Services.RequestHandlers;

public class GetLibrariesHandler(IPlayniteApi playniteApi) : RequestHandler<GetLibrariesRequest>
{
    public override async ValueTask<SatelightResponse> HandleAsync(GetLibrariesRequest request, CancellationToken token) => new GetLibrariesResponse 
    {
        Items = playniteApi.Library.Sources
                .Select(s => new Label { Id = s.Id, Name = s.Name })
                .ToArray()
    };

}
