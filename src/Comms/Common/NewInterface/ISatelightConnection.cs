using System;
using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface.Models;
using Common.Utility.Models;

namespace Comms.Common.Interface;

public interface ISatelightConnection : IDisposable, IAsyncDisposable
{
    Task<SatelightRequest> ReadRequestAsync(CancellationToken token);
    Task                   SendResponseAsync<T>(T response, CancellationToken token) where T : SatelightResponse;
    Conditional            IsConnected { get; }
}