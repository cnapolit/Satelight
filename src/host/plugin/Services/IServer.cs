namespace HostPlugin.Services;

public interface IServer : IAsyncDisposable
{
    void Run(CancellationToken token);
    Task RunAsync(CancellationToken token);
}
