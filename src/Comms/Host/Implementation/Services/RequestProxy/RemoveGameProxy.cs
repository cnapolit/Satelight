using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class RemoveGameProxy() : SimpleRequestProxy<RemoveGameRequest, RemoveGameBody, RemoveGameReply, RemoveGameResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.RemoveGame);