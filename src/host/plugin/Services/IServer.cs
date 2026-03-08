using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostPlugin.Services;

public interface IServer : IDisposable
{
    void Run(CancellationToken token);
    Task RunAsync(CancellationToken token);
}
