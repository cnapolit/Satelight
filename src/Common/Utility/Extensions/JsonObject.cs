using System.Text.Json.Nodes;
using System.Diagnostics.CodeAnalysis;
namespace Common.Utility.Extensions;

public static class JsonObjectExt
{
    public static bool IsTrue(this JsonObject obj, string propertyName)
        => obj.TryGet(propertyName, out bool value)
        && value;
    public static bool IsFalse(this JsonObject obj, string propertyName)
        => !IsTrue(obj, propertyName);
    public static bool TryGet<T>(this JsonObject obj, string propertyName, out T jsonT)
    {
         if (obj.TryGetPropertyValue(propertyName, out var value)
          && value is JsonValue jsonVal
          && jsonVal.TryGetValue(out jsonT))
         {
             return true;
         }
         jsonT = default;
         return false;
    }
}