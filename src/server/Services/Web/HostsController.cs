using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models.Database;

namespace Server.Services.Web;

[ApiController]
[Route("api/hosts")]
public class HostsController(IDbContextFactory<DatabaseContext> databaseContextFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HostDto>>> GetHosts(CancellationToken cancellationToken)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var hosts = await databaseContext
                         .Hosts
                         .AsNoTracking()
                         .OrderBy(h => h.DisplayName == string.Empty ? h.DnsName : h.DisplayName)
                         .ToListAsync(cancellationToken);

        var response = hosts.Select(h => new HostDto
                        {
                            Id = h.Id,
                            Port = h.Port,
                            // SunshinePort = h.SunshinePort,
                            Added = h.Added,
                            DisplayName = h.DisplayName,
                            DnsName = h.DnsName,
                            Ip = h.Ip,
                            MacAddress = h.MacAddress,
                            FormattedMacAddress = h.FormattedMacAddress,
                            OperatingSystem = h.OperatingSystem,
                            HostName = h.HostName
                        })
                        .ToList();
        return Ok(response);
    }

    [HttpGet("{hostId:guid}/games")]
    public async Task<ActionResult<IReadOnlyList<HostGameViewDto>>> GetHostGames(Guid hostId, CancellationToken cancellationToken)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var hostExists = await databaseContext.Hosts.AsNoTracking().AnyAsync(h => h.Id == hostId, cancellationToken);
        if (!hostExists)
        {
            return NotFound(new { message = $"Host '{hostId}' does not exist." });
        }

        var hostGames = await databaseContext
                              .HostGames
                              .AsNoTracking()
                              .Where(hg => hg.HostId == hostId)
                              .OrderBy(hg => hg.LibraryGame.GameVariant.Name)
                              .ThenBy(hg => hg.LibraryGame.GameVariant.SubName)
                              .Select(hg => new HostGameViewDto
                              {
                                  HostGame = new HostGameDto
                                  {
                                      Id = hg.Id,
                                      HostId = hg.HostId,
                                      LibraryGameId = hg.LibraryGameId,
                                      Installed = hg.Installed,
                                      Playing = hg.Playing,
                                      Size = hg.Size,
                                      HostGameId = hg.HostGameId,
                                      InstallPath = hg.InstallPath,
                                      Version = hg.Version
                                  },
                                  Game = hg.LibraryGame.GameVariant.Games
                                           .Select(g => new GameDto
                                           {
                                               Id = g.Id,
                                               Name = g.Name,
                                               SortingName = g.SortingName,
                                               Description = g.Description
                                           })
                                           .FirstOrDefault(),
                                  Variant = new GameVariantDto
                                  {
                                      Id = hg.LibraryGame.GameVariant.Id,
                                      Name = hg.LibraryGame.GameVariant.Name,
                                      SubName = hg.LibraryGame.GameVariant.SubName,
                                      Description = hg.LibraryGame.GameVariant.Description
                                  }
                              })
                              .ToListAsync(cancellationToken);
        return Ok(hostGames);
    }

    [HttpGet("{hostId:guid}/operations")]
    public async Task<ActionResult<IReadOnlyList<OperationDto>>> GetHostOperations(
        Guid hostId,
        [FromQuery] bool active = false,
        CancellationToken cancellationToken = default)
    {
        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var hostExists = await databaseContext.Hosts.AsNoTracking().AnyAsync(h => h.Id == hostId, cancellationToken);
        if (!hostExists)
        {
            return NotFound(new { message = $"Host '{hostId}' does not exist." });
        }

        var query = databaseContext
                    .Operations
                    .AsNoTracking()
                    .Where(o => o.HostId == hostId);

        if (active)
        {
            query = query.Where(
                o => o.State == OperationState.Queued || o.State == OperationState.Running || o.State == OperationState.Paused);
        }

        var operations = await query.OrderByDescending(o => o.Id)
                                    .Select(o => new OperationDto
                                    {
                                        Id = o.Id,
                                        HostOpId = o.HostOpId,
                                        State = o.State,
                                        Type = o.Type,
                                        Progress = o.Progress,
                                        HostId = o.HostId,
                                        TargetId = o.TargetId
                                    })
                                    .ToListAsync(cancellationToken);
        return Ok(operations);
    }

    [HttpPost("{hostId:guid}/operations")]
    public async Task<ActionResult<OperationDto>> QueueHostOperation(
        Guid hostId,
        [FromBody] QueueOperationRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.Type) || request.Type == OperationType.None)
        {
            return BadRequest(new { message = $"Operation type '{request.Type}' is not supported." });
        }

        await using var databaseContext = await databaseContextFactory.CreateDbContextAsync(cancellationToken);
        var hostExists = await databaseContext.Hosts.AsNoTracking().AnyAsync(h => h.Id == hostId, cancellationToken);
        if (!hostExists)
        {
            return NotFound(new { message = $"Host '{hostId}' does not exist." });
        }

        var targetId = request.TargetId;
        var gameOperation = IsGameOperation(request.Type);
        if (gameOperation)
        {
            var hostGameExists = await databaseContext
                                     .HostGames
                                     .AsNoTracking()
                                     .AnyAsync(hg => hg.Id == targetId && hg.HostId == hostId, cancellationToken);
            if (!hostGameExists)
            {
                return BadRequest(new { message = $"Host game '{targetId}' is not associated with host '{hostId}'." });
            }
        }
        else
        {
            targetId = targetId == Guid.Empty ? hostId : targetId;
        }

        var operation = new Operation
        {
            HostOpId = Guid.NewGuid(),
            HostId = hostId,
            TargetId = targetId,
            Type = request.Type,
            State = OperationState.Queued,
            Progress = 0
        };

        databaseContext.Operations.Add(operation);
        await databaseContext.SaveChangesAsync(cancellationToken);

        var response = new OperationDto
        {
            Id = operation.Id,
            HostOpId = operation.HostOpId,
            State = operation.State,
            Type = operation.Type,
            Progress = operation.Progress,
            HostId = operation.HostId,
            TargetId = operation.TargetId
        };

        return CreatedAtAction(nameof(GetHostOperations), new { hostId = operation.HostId }, response);
    }

    private static bool IsGameOperation(OperationType operationType)
        => operationType is OperationType.StartGame
                        or OperationType.MonitorGame
                        or OperationType.StopGame
                        or OperationType.InstallGame
                        or OperationType.UninstallGame
                        or OperationType.UpdateGame
                        or OperationType.RemoveGame
                        or OperationType.RepairGame
                        or OperationType.MoveGame;
}

public sealed class QueueOperationRequest
{
    public Guid TargetId { get; set; }
    public OperationType Type { get; set; }
}

public sealed class HostDto
{
    public Guid Id { get; set; }
    public short Port { get; set; }
    public ushort SunshinePort { get; set; }
    public DateTime Added { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string DnsName { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public string FormattedMacAddress { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
}

public sealed class GameDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SortingName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class GameVariantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SubName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class HostGameDto
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }
    public Guid LibraryGameId { get; set; }
    public bool Installed { get; set; }
    public bool Playing { get; set; }
    public long? Size { get; set; }
    public string HostGameId { get; set; } = string.Empty;
    public string InstallPath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

public sealed class HostGameViewDto
{
    public HostGameDto HostGame { get; set; } = new();
    public GameDto? Game { get; set; }
    public GameVariantDto? Variant { get; set; }
}

public sealed class OperationDto
{
    public Guid Id { get; set; }
    public Guid HostOpId { get; set; }
    public OperationState State { get; set; }
    public OperationType Type { get; set; }
    public short Progress { get; set; }
    public Guid HostId { get; set; }
    public Guid TargetId { get; set; }
}
