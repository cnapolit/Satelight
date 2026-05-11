using Makaretu.Dns;
using Microsoft.Extensions.Options;
using Service.Models;

namespace Service.Services;

public sealed class HostAdvertisementService(
    IOptions<HostAdvertisementOptions> options,
    ILogger<HostAdvertisementService> logger) : BackgroundService
{
    private const string ServiceName = "_satelight._tcp";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var opts = options.Value;
        if (opts.Port <= 0)
        {
            logger.LogWarning("Host advertisement disabled: port not configured.");
            return;
        }

        ServiceProfile profile = new(opts.InstanceName, ServiceName, (ushort)opts.Port);
        foreach (var (key, value) in opts.Metadata)
        {
            profile.AddProperty(key, value);
        }

        using ServiceDiscovery discovery = new();
        try
        {
            discovery.Advertise(profile);
            discovery.Announce(profile);
            logger.LogInformation(
                "Advertising '{Instance}' on {Service} port {Port}.",
                opts.InstanceName, ServiceName, opts.Port);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            try { discovery.Unadvertise(profile); }
            catch (Exception ex) { logger.LogWarning(ex, "Failed to unadvertise service."); }
        }
    }
}
