using System;
using Piped;
using ProtoBuf;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal abstract class SimpleRequestProxy<TIn, TReq, TRep, TRes>(
    Func<TIn, TReq> createRequest, Func<TRep, TRes> createResult, RequestType requestType)
    : SingleRequestProxy<TIn, TReq, TRep, TRes>
    where TReq : IExtensible
    where TRep : IExtensible
{
    protected sealed override TReq RequestConstructor(TIn input)
        => createRequest(input);
    protected sealed override TRes ResultConstructor(TRep reply)
        => createResult(reply);
    protected sealed override RequestType Type => requestType;
}