using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class StartGameProxy() : SimpleRequestProxy<StartGameRequest, StartGameBody, StartGameReply, StartGameResponse>(
        new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.StartGame);