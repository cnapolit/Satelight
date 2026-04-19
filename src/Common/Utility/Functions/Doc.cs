namespace Common.Utility.Functions;

public static class Doc
{
    public static bool TryGet(out string filePath, params string[] segments)
    {
        filePath = Path.Combine(segments);
        return File.Exists(filePath);
    }
}