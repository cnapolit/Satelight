using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Common.Utility.Models;

namespace Common.Utility.Extensions;

public static class IEnumerableExt
{
    public static IEnumerable<T> While<T>(this IEnumerable<T> source, params Conditional[] conditions)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var item in source)
        {
            if (!conditions.All())
            {
                yield break;
            }
            yield return item;
        }
    }

    public static IEnumerable<T> WithCancellation<T>(this IEnumerable<T> source, CancellationToken token)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var item in source)
        {
            if (token.IsCancellationRequested)
            {
                yield break;
            }
            yield return item;
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> func)
    {
        foreach (var item in source) await func(item);
    }

    public static async IAsyncEnumerable<T> WhereAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> func)
    {
        foreach (var item in source) if (await func(item)) yield return item;
    }

    public static async IAsyncEnumerable<T> WhereAsync<T>(
        this IEnumerable<T> source, Func<T, CancellationToken, Task<bool>> func, [EnumeratorCancellation] CancellationToken token)
    {
        foreach (var item in source) if (await func(item, token)) yield return item;
    }

    public static async IAsyncEnumerable<T> WhereAsync<T>(
        this IEnumerable<T> source, Func<T, CancellationToken, ValueTask<bool>> func, [EnumeratorCancellation] CancellationToken token)
    {
        foreach (var item in source) if (await func(item, token)) yield return item;
    }

    public static async IAsyncEnumerable<T2> SelectAsync<T, T2>(this IEnumerable<T> source, Func<T, Task<T2>> func)
    {
        foreach (var item in source) yield return await func(item);
    }

    public static async ValueTask WhenAllAsync(this IEnumerable<ValueTask> tasks)
    {
        foreach (var task in tasks) await task;
    }

    public static async Task WhenAllAsync(this IEnumerable<Task> tasks)
    {
        foreach (var task in tasks) await task;
    }

    public static bool All<TCon>(this IEnumerable<TCon> conditions) where TCon : Conditional
        => conditions.All(c => c);

    public static bool All(this IEnumerable<bool> conditions)
        => conditions.All(c => c);

    public static T[] AsArray<T>(this IEnumerable<T>? source) => source?.ToArray() ?? [];

    public static List<T> AsList<T>(this IEnumerable<T>? source) => source?.ToList() ?? [];

    public static IEnumerable<T> As<T>(this IEnumerable<object> source) where T : class
        => source.Where(o => o is T).Cast<T>();
}