using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Satelight.Protos.Host;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class InstallGameProxy() : SimpleRequestProxy<InstallGameRequest, InstallGameBody, InstallGameReply, InstallGameResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.InstallGame);