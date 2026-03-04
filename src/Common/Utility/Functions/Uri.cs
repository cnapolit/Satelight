using System;
using System.IO;

namespace Common.Utility.Functions;

public static class Uri
{
    public static bool IsNotRelative(string path, string parentDir)
        => !Path.GetFullPath(path).StartsWith(parentDir, StringComparison.OrdinalIgnoreCase);
}