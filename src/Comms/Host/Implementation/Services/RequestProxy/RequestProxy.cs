using Piped;
using ProtoBuf;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal abstract class RequestProxy
{
    public string IpAddress { private get; init; } = string.Empty;
    public int    Port      { private get; init; }

    protected static NamedPipeClientStream CreateClient()
        => new(".", "NetServer", PipeDirection.InOut, PipeOptions.Asynchronous);

    protected async Task<TRep> SendAsync<TReq, TRep>(RequestType type, CancellationToken token)
        where TReq : IExtensible, new() where TRep : IExtensible
    {
        using var client = CreateClient();
        await client.ConnectAsync(token);
        return await SendAsync<TReq, TRep>(client, type, token);
    }

    protected Task<TRep> SendAsync<TReq, TRep>(
        Stream client, RequestType type, CancellationToken token)
        where TReq : IExtensible, new() where TRep : IExtensible
        => SendAsync<TReq, TRep>(client, new(), type, token);

    protected async Task<TRep> SendAsync<TReq, TRep>(
        TReq request, RequestType type, CancellationToken token)
        where TReq : IExtensible where TRep : IExtensible
    {
        using var client = CreateClient();
        await client.ConnectAsync(token);
        return await SendAsync<TReq, TRep>(client, request, type, token);
    }

    protected async Task<TRep> SendAsync<TReq, TRep>(
        Stream client, TReq request, RequestType type, CancellationToken token)
        where TReq : IExtensible where TRep : IExtensible
    {
        await WriteMessageAsync(client, request, type, token);
        return await ReadMessageAsync<TRep>(client, token);
    }

    public async Task WriteMessageAsync<T>(Stream stream, T message, RequestType type, CancellationToken token)
        where T : IExtensible
    {
        //ServerRequest request;
        //using (MemoryStream messageStream = new())
        //{
        //    Serializer.Serialize(messageStream, message);
        //    request = new()
        //    {
        //        Type = type,
        //        Host = IpAddress,
        //        Port = Port,
        //        Parameters = messageStream.ToArray()
        //    };
        //}

        //byte[] data;
        //using (MemoryStream memoryStream = new())
        //{
        //    Serializer.Serialize(memoryStream, request);
        //    data = memoryStream.ToArray();
        //}
        //await stream.WriteAsync(data, 0, data.Length, token);
    }

    public static async Task<T> ReadMessageAsync<T>(Stream stream, CancellationToken token)
    {
        var buffer = new byte[1024];
        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
        using MemoryStream memoryStream = new();
        memoryStream.Write(buffer, 0, bytesRead);
        memoryStream.Position = 0;
        return Serializer.Deserialize<T>(memoryStream);
    }
}