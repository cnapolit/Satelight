using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class RepairGameProxy() : SimpleRequestProxy<RepairGameRequest, RepairGameBody, RepairGameReply, RepairGameResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.RepairGame);