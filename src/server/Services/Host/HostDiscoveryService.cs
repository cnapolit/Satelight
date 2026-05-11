using System.Net;
using Makaretu.Dns;
using Server.Models.Events;

namespace Server.Services.Host;

public sealed class HostDiscoveryService(ILogger<HostDiscoveryService> logger) : BackgroundService
{
    private const string ServiceName = "_satelight._tcp";
    private static readonly TimeSpan RequeryInterval = TimeSpan.FromSeconds(30);

    public event Func<HostDiscoveredEventArgs, Task>? HostDiscovered;
    public event Func<HostLostEventArgs, Task>? HostLost;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using ServiceDiscovery discovery = new();

        discovery.ServiceInstanceDiscovered += OnInstanceDiscovered;
        discovery.ServiceInstanceShutdown += OnInstanceShutdown;

        try
        {
            using PeriodicTimer timer = new(RequeryInterval);
            do
            {
                discovery.QueryServiceInstances(ServiceName);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        finally
        {
            discovery.ServiceInstanceDiscovered -= OnInstanceDiscovered;
            discovery.ServiceInstanceShutdown -= OnInstanceShutdown;
        }
    }

    private void OnInstanceDiscovered(object? sender, ServiceInstanceDiscoveryEventArgs e)
    {
        if (sender is not ServiceDiscovery discovery) return;
        if (!e.ServiceInstanceName.ToString().Contains(ServiceName, StringComparison.OrdinalIgnoreCase)) return;

        try
        {
            var instanceName = e.ServiceInstanceName.ToString();
            var srv = e.Message.AdditionalRecords.OfType<SRVRecord>().FirstOrDefault();
            var txt = e.Message.AdditionalRecords.OfType<TXTRecord>().FirstOrDefault();
            var addresses = e.Message.AdditionalRecords
                .OfType<AddressRecord>()
                .Select(a => a.Address)
                .Where(a => a is not null)
                .Cast<IPAddress>()
                .ToList();

            var hostName = srv?.Target.ToString() ?? string.Empty;
            var port = srv?.Port ?? 0;
            var metadata = ParseTxt(txt);

            HostDiscoveredEventArgs args = new(instanceName, hostName, addresses, port, metadata);
            _ = InvokeAsync(HostDiscovered, args, instanceName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to process discovered service instance.");
        }
    }

    private void OnInstanceShutdown(object? sender, ServiceInstanceShutdownEventArgs e)
    {
        if (!e.ServiceInstanceName.ToString().Contains(ServiceName, StringComparison.OrdinalIgnoreCase)) return;
        var instanceName = e.ServiceInstanceName.ToString();
        _ = InvokeAsync(HostLost, new HostLostEventArgs(instanceName), instanceName);
    }

    private async Task InvokeAsync<T>(Func<T, Task>? handler, T args, string instanceName)
    {
        if (handler is null) return;
        try
        {
            await handler.Invoke(args);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Host discovery handler threw for instance '{Instance}'.", instanceName);
        }
    }

    private static IReadOnlyDictionary<string, string> ParseTxt(TXTRecord? record)
    {
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);
        if (record is null) return result;

        foreach (var entry in record.Strings)
        {
            var idx = entry.IndexOf('=');
            if (idx <= 0) continue;
            result[entry[..idx]] = entry[(idx + 1)..];
        }
        return result;
    }
}
