using Comms.Host.Interface.Models;
using Piped;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class UpdateGameProxy() : SimpleRequestProxy<UpdateGameRequest, UpdateGameBody, UpdateGameReply, UpdateGameResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, RequestType.UpdateGame);