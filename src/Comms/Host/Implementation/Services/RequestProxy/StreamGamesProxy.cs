using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Comms.Common.Interface.Models;

namespace Comms.Host.Implementation.Services.RequestProxy;

internal class StreamGamesProxy : RequestProxy
{
    public async IAsyncEnumerable<StreamGamesResponse> StreamGamesAsync(
        StreamGamesRequest request, [EnumeratorCancellation] CancellationToken token)
    {
        yield break;
        using var client = CreateClient();
        await client.ConnectAsync(token);
        //await WriteMessageAsync(client, HostModelMapper.Map(request), RequestType.StreamGames, token);
        //do
        //{
        //    var game = await ReadMessageAsync<Satelight.Protos.Host.Game>(client, token);
        //    yield return HostModelMapper.Map(game);
        //} while (client.IsConnected && !token.IsCancellationRequested);
    }
}