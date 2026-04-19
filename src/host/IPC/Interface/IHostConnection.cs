using Comms.Common.Interface.Models;
using Common.Utility.Models;

namespace Comms.Host.Interface;

public interface IHostConnection : IDisposable, IAsyncDisposable
{
    Task<SatelightRequest> ReadRequestAsync(CancellationToken token);
    Task                   SendResponseAsync<T>(T response, CancellationToken token) where T : SatelightResponse;
    Conditional            IsConnected { get; }
}
