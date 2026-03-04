using Playnite.SDK;
using System;
using System.Threading;
using Comms.Host.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class UpdateGameRequestHandler
{
    public UpdateGameResponse Handle(
        IPlayniteAPI playniteApi, UpdateGameRequest request, CancellationToken token)
        => throw new NotImplementedException();
}