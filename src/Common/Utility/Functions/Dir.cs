using System.IO;

namespace Common.Utility.Functions;

public static class Dir
{
    public static bool TryGet(out string dirPath, params string[] segments)
    {
        dirPath = Path.Combine(segments);
        return Directory.Exists(dirPath);
    }
}