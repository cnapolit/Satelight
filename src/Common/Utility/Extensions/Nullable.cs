using System;

namespace Common.Utility.Extensions;

public static class NullableExt
{
    public static T AsValue<T>(this T? value) where T : struct => value.GetValueOrDefault();
}