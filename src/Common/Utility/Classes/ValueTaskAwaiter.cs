#if NET5_0_OR_GREATER
#else
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endif
using Common.Utility.Extensions;

namespace Common.Utility.Classes;

public class ValueTaskAwaiter(params IEnumerable<ValueTask> valueTasks)
{
  private bool _awaited;

  public static ValueTaskAwaiter WhenAll(params IEnumerable<ValueTask> valueTasks) => new(valueTasks);

  public async Task WithAsync(params IEnumerable<Task> tasks)
  {
    lock (this)
    {
      if (_awaited) throw new InvalidOperationException($"{nameof(ValueTaskAwaiter)} was already awaited");
      _awaited = true;
    }
    await tasks.WhenAllAsync();
    await valueTasks.WhenAllAsync();
  }
}