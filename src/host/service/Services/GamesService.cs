using Grpc.Core;
using Satelight.Protos.Host;
using Service.Common;
using System.IO.Pipes;
using Google.Protobuf;
using Piped;
using System.Buffers;
using Service.Common.Extensions;

namespace Service.Services;

public class GamesService(ILogger<GamesService> logger) : Games.GamesBase
{
    public override async Task Stream(StreamGamesBody body, IServerStreamWriter<Game> responseStream, ServerCallContext context)
    {;
        await using NamedPipeClientStream client = new(".", "SatelightHost", PipeDirection.InOut, PipeOptions.Asynchronous);
        await client.ConnectAsync(context.CancellationToken);
        Body msg = new() { Type = RequestType.StreamGames, Parameters = body.ToByteString() };
        await client.WriteAsync(msg.ToByteArray(), context.CancellationToken);

        Memory<byte> memory = new byte[4096];
        Memory<byte> lengthBuffer = new byte[4];
        do
        {
            var read = await client.ReadAsync(lengthBuffer, context.CancellationToken);
            if (read != 4) throw new EndOfStreamException();
            var messageLength = BitConverter.ToInt32(lengthBuffer.ToArray(), 0);

            if (memory.Length < messageLength) memory = new byte[messageLength];

            var totalBytesRead = 0;
            do
            {
                totalBytesRead += await client.ReadAsync(memory, context.CancellationToken);
            } while (totalBytesRead < messageLength);
            var game = Game.Parser.ParseFrom(memory, totalBytesRead);
            await responseStream.WriteAsync(game, context.CancellationToken);
        } while(client.IsConnected);
    }

    public override Task<UpdateGameReply> Update(UpdateGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<UpdateGameReply>(
            logger, RequestType.UpdateGame, request, UpdateGameReply.Parser, context.CancellationToken);

    public override Task<CountGamesReply> Count(CountGamesBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<CountGamesReply>(
            logger, RequestType.CountGames, request, CountGamesReply.Parser, context.CancellationToken);

    public override Task<GetGameReply> Get(GetGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<GetGameReply>(
            logger, RequestType.GetGame, request, GetGameReply.Parser, context.CancellationToken);

    public override Task<UninstallGameReply> Uninstall(UninstallGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<UninstallGameReply>(
            logger, RequestType.UninstallGame, request, UninstallGameReply.Parser, context.CancellationToken);

    public override Task<RemoveGameReply> Remove(RemoveGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<RemoveGameReply>(
            logger, RequestType.RemoveGame, request, RemoveGameReply.Parser, context.CancellationToken);

    public override Task<RepairGameReply> Repair(RepairGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<RepairGameReply>(
            logger, RequestType.RepairGame, request, RepairGameReply.Parser, context.CancellationToken);

    public override Task<InstallGameReply> Install(InstallGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<InstallGameReply>(
            logger, RequestType.InstallGame, request, InstallGameReply.Parser, context.CancellationToken);

    public override Task<StartGameReply> Start(StartGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<StartGameReply>(
            logger, RequestType.StartGame, request, StartGameReply.Parser, context.CancellationToken);

    public override Task<StopGameReply> Stop(StopGameBody request, ServerCallContext context)
        => Pipe.SendRequestAsync<StopGameReply>(
            logger, RequestType.StopGame, request, StopGameReply.Parser, context.CancellationToken);
}