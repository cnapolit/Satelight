using Google.Protobuf;
using Piped;
using Service.Common.Extensions;
using System.IO.Pipes;

namespace Service.Common;

public static class Pipe
{
    public static async Task<T> SendRequestAsync<T>(ILogger logger, RequestType type, IMessage request, MessageParser<T> parser, CancellationToken token)
        where T : IMessage<T>, new()
    {
        logger.Log(LogLevel.Information, $"Sending message of type '{type}'");
        return await SendRequestAsync(type, request, parser, token);
    }

    public static async Task<TOut> SendRequestAsync<TOut>(
        RequestType type, IMessage request, MessageParser<TOut> parser, CancellationToken token)
        where TOut : IMessage<TOut>, new()
    {
        Body msg = new() { Type = type, Parameters = request.ToByteString() };
        await using NamedPipeClientStream client = new(".", "SatelightHost", PipeDirection.InOut, PipeOptions.Asynchronous);
        await client.ConnectAsync(token);
        await client.WriteAsync(msg.ToByteArray(), token);
        Memory<byte> memory = new byte[1024 * 1024 * 4];
        var bytesRead = await client.ReadAsync(memory, token);
        return parser.ParseFrom(memory, bytesRead);
    }
}