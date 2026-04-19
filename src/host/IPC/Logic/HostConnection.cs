using System.IO.Pipes;
using Comms.Common.Interface.Models;
using Common.Utility.Models;
using Comms.Common.Implementation.Services;
using Comms.Host.Interface;
using Comms.Host.Interface.Models;
using Satelight.Protos.Core;
using Satelight.Protos.Host;
using Piped;
using RequestType = Piped.RequestType;
using Google.Protobuf;

namespace Comms.Host.Implementation;

public sealed class HostConnection(NamedPipeServerStream stream) : IHostConnection
{
    private readonly ModelMapper _mapper = new();

    public void Dispose() => stream.Dispose();

    public ValueTask DisposeAsync() => stream.DisposeAsync();

    public async Task<SatelightRequest> ReadRequestAsync(CancellationToken token)
    {
        var buffer = new byte[1024];
        var bytesRead = await stream.ReadAsync(buffer, token);
        var request = Body.Parser.ParseFrom(buffer, 0, bytesRead);
        using MemoryStream paramStream = new(request.Parameters.ToByteArray());
        return MapRequest(request.Type, paramStream);
    }

    private SatelightRequest MapRequest(RequestType type, MemoryStream paramStream) => type switch
    {
        RequestType.GetGameCover         => _mapper.Map(GetGameCoversBody.Parser.ParseFrom(paramStream)),
        RequestType.GetGameBackground    => _mapper.Map(GetGameBackgroundsBody.Parser.ParseFrom(paramStream)),
        RequestType.GetGame              => _mapper.Map(GetGameBody.Parser.ParseFrom(paramStream)),
        RequestType.StreamGames          => _mapper.Map(StreamGamesBody.Parser.ParseFrom(paramStream)),
        RequestType.CountGames           => _mapper.Map(CountGamesBody.Parser.ParseFrom(paramStream)),
        RequestType.StartGame            => _mapper.Map(StartGameBody.Parser.ParseFrom(paramStream)),
        RequestType.StopGame             => _mapper.Map(StopGameBody.Parser.ParseFrom(paramStream)),
        RequestType.InstallGame          => _mapper.Map(InstallGameBody.Parser.ParseFrom(paramStream)),
        RequestType.UninstallGame        => _mapper.Map(UninstallGameBody.Parser.ParseFrom(paramStream)),
        RequestType.UpdateGame           => _mapper.Map(UpdateGameBody.Parser.ParseFrom(paramStream)),
        RequestType.RemoveGame           => _mapper.Map(RemoveGameBody.Parser.ParseFrom(paramStream)),
        RequestType.RepairGame           => _mapper.Map(RepairGameBody.Parser.ParseFrom(paramStream)),
        RequestType.MoveGame             => _mapper.Map(MoveGameBody.Parser.ParseFrom(paramStream)),
        RequestType.UpdateGameCover      => _mapper.Map(UpdateCoverBody.Parser.ParseFrom(paramStream)),
        RequestType.UpdateGameBackground => _mapper.Map(UpdateBackgroundBody.Parser.ParseFrom(paramStream)),
        RequestType.Init                 => _mapper.Map(InitBody.Parser.ParseFrom(paramStream)),
        RequestType.GetCache             => _mapper.Map(GetCacheBody.Parser.ParseFrom(paramStream)),
        RequestType.GetOp                => _mapper.Map(GetOpBody.Parser.ParseFrom(paramStream)),
        RequestType.ListOps              => _mapper.Map(ListOpsBody.Parser.ParseFrom(paramStream)),
        RequestType.ListTags             => _mapper.Map(ListTagsBody.Parser.ParseFrom(paramStream)),
        RequestType.ListPlatforms        => _mapper.Map(ListPlatformsBody.Parser.ParseFrom(paramStream)),
        RequestType.ListGenres           => _mapper.Map(ListGenresBody.Parser.ParseFrom(paramStream)),
        RequestType.ListLibraries        => _mapper.Map(ListLibrariesBody.Parser.ParseFrom(paramStream)),
        RequestType.ListSeries           => _mapper.Map(ListSeriesBody.Parser.ParseFrom(paramStream)),
        RequestType.ListFeatures         => _mapper.Map(ListFeaturesBody.Parser.ParseFrom(paramStream)),
        RequestType.ListCompanies        => _mapper.Map(ListCompaniesBody.Parser.ParseFrom(paramStream)),
        _                                => throw new ArgumentOutOfRangeException()
    };

    public async Task SendResponseAsync<T>(T response, CancellationToken token) where T : SatelightResponse
    {
        var reply = MapResponse(response).ToByteArray();
        if (response is StreamGamesResponse)
            await stream.WriteAsync(BitConverter.GetBytes(reply.Length), 0, 4, token);
        await stream.WriteAsync(reply, 0, reply.Length, token);
    }

    private IMessage MapResponse<T>(T response) where T : SatelightResponse => response switch
    {
        GetGameCoverResponse           getGameCoverResponse => _mapper.Map     (getGameCoverResponse),
        GetGameBackgroundResponse getGameBackgroundResponse => _mapper.Map(getGameBackgroundResponse),
        StartGameResponse                 startGameResponse => _mapper.Map        (startGameResponse),
        StopGameResponse                   stopGameResponse => _mapper.Map         (stopGameResponse),
        GetGameResponse                     getGameResponse => _mapper.Map          (getGameResponse),
        StreamGamesResponse             streamGamesResponse => _mapper.Map      (streamGamesResponse),
        CountGamesResponse               countGamesResponse => _mapper.Map       (countGamesResponse),
        InstallGameResponse             installGameResponse => _mapper.Map      (installGameResponse),
        UninstallGameResponse         uninstallGameResponse => _mapper.Map    (uninstallGameResponse),
        UpdateGameResponse               updateGameResponse => _mapper.Map       (updateGameResponse),
        RemoveGameResponse               removeGameResponse => _mapper.Map       (removeGameResponse),
        RepairGameResponse               repairGameResponse => _mapper.Map       (repairGameResponse),
        MoveGameResponse                   moveGameResponse => _mapper.Map         (moveGameResponse),
        UpdateCoverResponse             updateCoverResponse => _mapper.Map      (updateCoverResponse),
        UpdateBackgroundResponse   updateBackgroundResponse => _mapper.Map (updateBackgroundResponse),
        InitializeResponse                     initResponse => _mapper.Map             (initResponse),
        GetCacheResponse                   getCacheResponse => _mapper.Map         (getCacheResponse),
        GetOpResponse                         getOpResponse => _mapper.Map            (getOpResponse),
        GetOpsResponse                       getOpsResponse => _mapper.Map           (getOpsResponse),
        GetTagsResponse                     getTagsResponse => _mapper.Map          (getTagsResponse),
        GetPlatformsResponse           getPlatformsResponse => _mapper.Map     (getPlatformsResponse),
        GetGenresResponse                 getGenresResponse => _mapper.Map        (getGenresResponse),
        GetLibrariesResponse           getLibrariesResponse => _mapper.Map     (getLibrariesResponse),
        GetSeriesResponse                 getSeriesResponse => _mapper.Map        (getSeriesResponse),
        GetFeaturesResponse             getFeaturesResponse => _mapper.Map      (getFeaturesResponse),
        GetCompaniesResponse           getCompaniesResponse => _mapper.Map     (getCompaniesResponse),
        _                                                   => throw new ArgumentOutOfRangeException()
    };

    public Conditional IsConnected { get; } = new FuncConditional(() => stream.IsConnected);
}
