using System.Buffers;
using Google.Protobuf;

namespace Service.Common.Extensions;

public static class MessageParserExt
{
    public static T ParseFrom<T>(this MessageParser<T> parser, Memory<byte> data, int bytesRead) where T : IMessage<T>, new()
    {
        return parser.ParseFrom(new ReadOnlySequence<byte>(data[..bytesRead]));
    }
}