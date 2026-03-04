//using Database;
//using Games;
//using Google.Protobuf;
//using Grpc.Core;
//using Grpc.Net.Client;
//using Piped;
//using System.IO.Pipes;
//using static Games.Games;

//namespace Service.Services;

//public class NamedPipeService(ILogger<NamedPipeService> logger) : BackgroundService
//{
//    private readonly Dictionary<string, GrpcChannel> _channels = [];
//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                while (!stoppingToken.IsCancellationRequested)
//                {
//                    await PollAsync(stoppingToken);
//                }
//            }
//            catch (Exception e)
//            {
//                logger.LogError(e, "Exception occurred while polling");
//            }
//        }
//    }

//    private async Task PollAsync(CancellationToken stoppingToken)
//    {
//        await using NamedPipeServerStream server = new("NetServer", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
//        await server.WaitForConnectionAsync(stoppingToken);

//        Memory<byte> memory = new byte[4096];
//        var bytesRead = await server.ReadAsync(memory, stoppingToken);
//        var request = ServerRequest.Parser.ParseFrom(memory.Span[..bytesRead]);

//        if (!_channels.TryGetValue(request.Host, out var channel))
//        {
//            channel = _channels[request.Host] = GrpcChannel.ForAddress($"https://{request.Host}:{request.Port}");
//        }

//        GamesClient gamesClient = new(channel);
//        Database.Database.DatabaseClient databaseClient = new(channel);
//        switch (request.Type)
//        {
//            case RequestType.Init:
//            case RequestType.GetGameIds:
//            case RequestType.RepairGame:
//            case RequestType.MoveGame:
//            default:
//                logger.LogWarning($"Received unknown request type {request.Type}");
//                break;
//            case RequestType.StreamGames:
//                var parameters = GamesStreamRequest.Parser.ParseFrom(request.Parameters);
//                var stream = gamesClient.StreamGame(parameters, cancellationToken: stoppingToken);
//                while (await stream.ResponseStream.MoveNext(stoppingToken))
//                {
//                    await server.WriteAsync(stream.ResponseStream.Current.ToByteArray(), stoppingToken);
//                }
//                break;
//            case RequestType.GetCache:
//                await WriteAsync(server, request.Parameters, GetCacheRequest.Parser, databaseClient.GetCacheAsync, stoppingToken);
//                break;
//            case RequestType.GetTags:
//                await WriteAsync(server, request.Parameters, GetLabelsRequest.Parser, databaseClient.GetTagsAsync, stoppingToken);
//                break;
//            case RequestType.GetGenres:
//                await WriteAsync(server, request.Parameters, GetLabelsRequest.Parser, databaseClient.GetGenresAsync, stoppingToken);
//                break;
//            case RequestType.GetGames:
//                await WriteAsync(server, request.Parameters, GamesGetRequest.Parser, gamesClient.GetAsync, stoppingToken);
//                break;
//            case RequestType.GetSources:
//                await WriteAsync(server, request.Parameters, GetLabelsRequest.Parser, databaseClient.GetSourcesAsync, stoppingToken);
//                break;
//            case RequestType.GetPlatforms:
//                await WriteAsync(server, request.Parameters, GetLabelsRequest.Parser, databaseClient.GetPlatformsAsync, stoppingToken);
//                break;
//            case RequestType.GetFilters:
//                await WriteAsync(server, request.Parameters, FiltersGetRequest.Parser, databaseClient.GetFiltersAsync, stoppingToken);
//                break;
//            case RequestType.SetFilter:
//                await WriteAsync(server, request.Parameters, SetFilterRequest.Parser, databaseClient.SetFilterAsync, stoppingToken);
//                break;
//            case RequestType.InstallGame:
//                await WriteAsync(server, request.Parameters, InstallRequest.Parser, gamesClient.InstallAsync, stoppingToken);
//                break;
//            case RequestType.UninstallGame:
//                await WriteAsync(server, request.Parameters, UninstallRequest.Parser, gamesClient.UninstallAsync, stoppingToken);
//                break;
//            case RequestType.UpdateGame:
//                await WriteAsync(server, request.Parameters, GamesUpdateRequest.Parser, gamesClient.UpdateAsync, stoppingToken);
//                break;
//            case RequestType.RemoveGame:
//                await WriteAsync(server, request.Parameters, RemoveRequest.Parser, gamesClient.RemoveAsync, stoppingToken);
//                break;
//            case RequestType.StartGame:
//                await WriteAsync(server, request.Parameters, StartRequest.Parser, gamesClient.StartAsync, stoppingToken);
//                break;
//            case RequestType.StopGame:
//                await WriteAsync(server, request.Parameters, StopRequest.Parser, gamesClient.StopAsync, stoppingToken);
//                break;
//            case RequestType.GetOps:
//                await WriteAsync(server, request.Parameters, GetOpRequest.Parser, gamesClient.GetOpAsync, stoppingToken);
//                break;
//        }

//        server.Disconnect();
//    }

//    private static async Task WriteAsync<TReq, TRep>(
//        NamedPipeServerStream server,
//        ByteString parameters,
//        MessageParser<TReq> parser,
//        Func<TReq, Metadata?, DateTime?, CancellationToken, AsyncUnaryCall<TRep>> func,
//        CancellationToken token)
//        where TReq : IMessage<TReq>
//        where TRep : IMessage<TRep>
//    {
//        var reply = await func(parser.ParseFrom(parameters), null, null, token);
//        await server.WriteAsync(reply.ToByteArray(), token);
//    }
//}