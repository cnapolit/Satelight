using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;

namespace HostPlugin.Common.Extensions;

public static class StreamExtensions
{
    public static async Task<T> ReadMessageAsync<T>(this Stream stream, CancellationToken token)
    {
        using var memoryStream = new MemoryStream();
        var buffer = new byte[1024];
        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
        memoryStream.Write(buffer, 0, bytesRead);
        memoryStream.Position = 0;
        return Serializer.Deserialize<T>(memoryStream);
    }

    public static async Task WriteMessageAsync<T>(this Stream stream, T message, CancellationToken token)
    {
        using var memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, message);
        var data = memoryStream.ToArray();
        await stream.WriteAsync(memoryStream.ToArray(), 0, data.Length, token);
    }
}