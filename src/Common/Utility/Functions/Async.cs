#if NET5_0_OR_GREATER
#else
using System;
using System.Threading;
using System.Threading.Tasks;
#endif
namespace Common.Utility.Functions;

public static class Async
{
    public static async Task<TOut> TryAsync<TIn, TOut>(
        Func<TIn, CancellationToken, Task<TOut>> func, TIn input, CancellationToken token) where TOut : struct
    {
        TOut output = default;
        try { output = await func(input, token); } catch {}
        return output;
    }

    public static async ValueTask WhenAll(params ValueTask[] tasks)
    {
        foreach (var task in tasks) await task;
    }
}