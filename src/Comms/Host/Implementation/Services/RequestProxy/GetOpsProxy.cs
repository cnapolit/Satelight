using Comms.Common.Interface.Models;
using Satelight.Protos.Core;
using GetOpsRequest = Comms.Common.Interface.Models.GetOpsRequest;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class GetOpsProxy() : SimpleRequestProxy<GetOpsRequest, ListOpsBody, ListOpsReply, GetOpsResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.ListOps);