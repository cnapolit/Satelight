using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Utility.Extensions;

public static class IAsyncEnumerableExt
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var results = new List<T>();
        await foreach (var item in source) results.Add(item);
        return results;
    }
}