using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class MoveGameProxy() : SimpleRequestProxy<MoveGameRequest, MoveGameBody, MoveGameReply, MoveGameResponse>(null,null,Piped.RequestType.MoveGame);
//new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.RepairGame);