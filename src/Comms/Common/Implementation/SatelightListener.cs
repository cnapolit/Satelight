using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface;

namespace Comms.Common.Implementation;

public abstract class SatelightListener<TConn> : ISatelightListener<TConn> where TConn : ISatelightConnection
{
    public void Dispose() { }

    public async Task<TConn> WaitForConnectionAsync(CancellationToken token)
    {
        NamedPipeServerStream stream = new(PipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances);
        await stream.WaitForConnectionAsync(token);
        return CreateConnection(stream);
    }

    protected abstract string PipeName { get; }
    protected abstract TConn  CreateConnection(NamedPipeServerStream stream);
}