using System;
using Comms.Common.Interface.Models;
using Satelight.Protos.Core;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class GetOpProxy
    : SingleRequestProxy<GetOpRequest, GetOpBody, GetOpReply, GetOpResponse>
{
    private Guid RequestId { get; set; }

    protected override GetOpResponse ResultConstructor(GetOpReply reply)
    {
        var response = new HostModelMapper().Map(reply);
        response.Op.Id = RequestId;
        return response;
    }

    protected override GetOpBody RequestConstructor(GetOpRequest input)
    {
        RequestId = input.Id;
        return new HostModelMapper().Map(input);
    }

    protected override Piped.RequestType Type => Piped.RequestType.GetOp;
}