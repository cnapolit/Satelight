namespace Comms.Host.Interface;

public interface IHostListener : IDisposable
{
    Task<IHostConnection> WaitForConnectionAsync(CancellationToken token);
}
