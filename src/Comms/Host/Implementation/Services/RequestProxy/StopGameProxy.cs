using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class StopGameProxy() : SimpleRequestProxy<StopGameRequest, StopGameBody, StopGameReply, StopGameResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.StopGame);