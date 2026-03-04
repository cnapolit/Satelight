using Comms.Host.Interface.Models;
using Piped;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class UninstallGameProxy() : SimpleRequestProxy<UninstallGameRequest, UninstallGameBody, UninstallGameReply, UninstallGameResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, RequestType.UninstallGame);