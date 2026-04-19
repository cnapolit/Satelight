using System.IO.Pipes;
using Comms.Host.Interface;

namespace Comms.Host.Implementation;

public class HostListener : IHostListener
{
    public void Dispose() { }

    public async Task<IHostConnection> WaitForConnectionAsync(CancellationToken token)
    {
        NamedPipeServerStream stream = new("SatelightHost", PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances);
        await stream.WaitForConnectionAsync(token);
        return new HostConnection(stream);
    }
}
