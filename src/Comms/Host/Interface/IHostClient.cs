using System.Threading;
using System.Threading.Tasks;
using Comms.Common.Interface;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;

namespace Comms.Host.Interface;

public interface IHostClient : ISatelightClient
{
    Task<StartGameResponse>     StartGameAsync     (StartGameRequest     request, CancellationToken token);
    Task<StopGameResponse>      StopGameAsync      (StopGameRequest      request, CancellationToken token);
    Task<UpdateGameResponse>    UpdateGameAsync    (UpdateGameRequest    request, CancellationToken token);
    Task<RepairGameResponse>    RepairGameAsync    (RepairGameRequest    request, CancellationToken token);
    Task<MoveGameResponse>      MoveGameAsync      (MoveGameRequest      request, CancellationToken token);
    Task<InstallGameResponse>   InstallGameAsync   (InstallGameRequest   request, CancellationToken token);
    Task<UninstallGameResponse> UninstallGameAsync (UninstallGameRequest request, CancellationToken token);
    Task<RemoveGameResponse>    RemoveGameAsync    (RemoveGameRequest    request, CancellationToken token);
}