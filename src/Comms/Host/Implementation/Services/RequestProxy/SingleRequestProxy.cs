using ProtoBuf;
using System.Threading;
using System.Threading.Tasks;
using Piped;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal abstract class SingleRequestProxy<TIn, TReq, TRep, TRes> : RequestProxy, IRequestProxy<TIn, TRes>
    where TReq : IExtensible
    where TRep : IExtensible
{
    protected abstract TRes ResultConstructor(TRep reply);
    protected abstract TReq RequestConstructor(TIn input);

    protected abstract RequestType Type { get; }

    public async Task<TRes> SendAsync(TIn input, CancellationToken token)
        => ResultConstructor(await SendAsync<TReq, TRep>(RequestConstructor(input), Type, token));
}