using System;
using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public class GetLabelsRequestHandler<TResp>(Func<Label[]> getLabels) where TResp : GetLabelsResponse, new()
{
    public TResp Handle() => new() { Items = getLabels() };
}