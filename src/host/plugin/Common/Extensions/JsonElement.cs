using System.Text.Json;

namespace Service.Common.Extensions;

public static class JsonElementExt
{
    public static bool TryGetArr(this JsonElement json, string name, out JsonElement.ArrayEnumerator arrayEnumerator)
    {
        var result = json.TryGetProperty(name, out var items) && items.ValueKind is JsonValueKind.Array;
        arrayEnumerator = result ? items.EnumerateArray() : default;
        return result;
    }

    public static JsonElement.ArrayEnumerator AsArr(this JsonElement json, string name)
        => json.TryGetArr(name, out var items) ? items : [];

    public static bool TryGetStr(this JsonElement json, string name, out string str)
    {
        var result = json.TryGetProperty(name, out var prop) && prop.ValueKind is JsonValueKind.String;
        str = result ? prop.GetString()! : string.Empty;
        return result;
    }

    public static bool IsTrue(this JsonElement json, string name)
        => json.TryGetProperty(name, out var prop) && prop.ValueKind is JsonValueKind.True;
    public static bool IsFalse(this JsonElement json, string name)
        => json.TryGetProperty(name, out var prop) && prop.ValueKind is JsonValueKind.False;
}
