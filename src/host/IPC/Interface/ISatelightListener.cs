using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Comms.Server.Interface")]
[assembly: InternalsVisibleTo("Comms.Host.Interface")]
namespace Comms.Common.Interface;


public interface ISatelightListener<IConnection> : IDisposable where IConnection : ISatelightConnection
{
    Task<IConnection> WaitForConnectionAsync(CancellationToken token);
}