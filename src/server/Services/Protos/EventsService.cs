using System.Collections.Concurrent;
using Grpc.Core;
using Satelight.Protos.Events;

namespace Server.Services.Protos;

public class EventsService(ILogger<EventsService> logger) : Events.EventsBase
{
    private readonly ConcurrentDictionary<Guid, (IServerStreamWriter<EventMessage> stream, ServerCallContext context)> _subscribers = new();

    public override async Task Subscribe(SubscribeBody request, IServerStreamWriter<EventMessage> responseStream, ServerCallContext context)
    {
        var id = Guid.NewGuid();
        _subscribers.TryAdd(id, (responseStream, context));
        try   { await Task.Delay(Timeout.Infinite, context.CancellationToken); }
        catch (TaskCanceledException) { }
        finally
        {
            _subscribers.TryRemove(id, out _);
        }
    }

    public async Task PublishAsync(string eventType, string payload)
    {
        var message = new EventMessage { EventType = eventType, Payload = payload };
        foreach (var (stream, ctx) in _subscribers.Values)
        if      (!ctx.CancellationToken.IsCancellationRequested)
        try     { await stream.WriteAsync(message); } catch { }
    }
}
