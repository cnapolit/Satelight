using Google.Protobuf;

namespace Common.Utility.Extensions;

public static class ByteStringExt
{
    public static Guid ToGuid(this ByteString byteString) => new(byteString.ToByteArray());
}
