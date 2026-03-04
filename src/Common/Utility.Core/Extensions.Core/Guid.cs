using Google.Protobuf;

namespace Common.Utility.Extensions;

public static class GuidExt
{
    public static ByteString ToByteString(this Guid guid) => ByteString.CopyFrom(guid.ToByteArray());
}
