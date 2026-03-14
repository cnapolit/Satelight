using System;
using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface.Models;

namespace HostPlugin.Services.RequestHandlers;

public abstract class GetLabelsRequestHandler<TReq, TRep> : RequestHandler<TReq>
    where TReq : SatelightRequest
    where TRep : GetLabelsResponse, new()
{
    public IDatabaseService DatabaseService { protected get; set; }
    public override async ValueTask<SatelightResponse> HandleAsync(TReq request, CancellationToken token)
        => new TRep { Items = GetLabels() };

    protected abstract Label[] GetLabels();
}