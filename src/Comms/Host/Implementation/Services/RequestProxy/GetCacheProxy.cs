using Comms.Common.Interface.Models;
using Satelight.Protos.Core;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class GetCacheProxy() : SimpleRequestProxy<GetCacheRequest, GetCacheBody, GetCacheReply, GetCacheResponse>(
    new HostModelMapper().Map, new HostModelMapper().Map, Piped.RequestType.GetCache);