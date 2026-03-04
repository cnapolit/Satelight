using System.Collections.Concurrent;
using Grpc.Net.Client;

namespace Server.Services.Host;

public class ChannelManager
{
    private readonly ConcurrentDictionary<Guid, GrpcChannel> _hostChannels = [];

    public GrpcChannel GetChannel(Models.Database.Host host)
        => _hostChannels.TryGetValue(host.Id, out var channel) 
         ? channel
         : _hostChannels[host.Id] = GrpcChannel.ForAddress($"https://{host.Ip}:{host.Port}", new GrpcChannelOptions { 
                HttpHandler = new HttpClientHandler
                {
                    //TODO: fix
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                }
           });
}
