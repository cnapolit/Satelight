 using Comms.Common.Interface.Models;
 using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class GetGameProxy() : SimpleRequestProxy<GetGameRequest, GetGameBody, GetGameReply, GameResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.GetGame);