using Server.Models.UserInterface;

namespace Server.Services;

public interface IDatabaseClient
{
    Task<List<GameDisplayInfo>> GetGamesAsync(CancellationToken cancellationToken = default);
    Task<GameInfo> GetGameInfoAsync(string id, CancellationToken cancellationToken = default);
    Task StartGameAsync(string id, CancellationToken cancellationToken = default);
    Task StopGameAsync(string id, CancellationToken cancellationToken = default);
    Task InstallGameAsync(string id, CancellationToken cancellationToken = default);
    Task UninstallGameAsync(string id, CancellationToken cancellationToken = default);
    Task<HostInfo> GetHostAsync(CancellationToken cancellationToken = default);
}