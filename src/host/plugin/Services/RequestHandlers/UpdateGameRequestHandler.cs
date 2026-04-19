using Playnite;
using Comms.Host.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateGameRequestHandler
{
    public UpdateGameResponse Handle(
        IPlayniteApi playniteApi, UpdateGameRequest request, CancellationToken token)
        => throw new NotImplementedException();
}