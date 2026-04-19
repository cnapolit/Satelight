using Common.Utility.Extensions;
using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Piped;
using Satelight.Protos.Core;
using Satelight.Protos.Host;
using Game = Satelight.Protos.Host.Game;
using GetCacheRequest = Comms.Common.Interface.Models.GetCacheRequest;
using GetOpRequest = Comms.Common.Interface.Models.GetOpRequest;
using GetOpsRequest = Comms.Common.Interface.Models.GetOpsRequest;
using Label = Comms.Common.Interface.Models.Label;
using Op = Comms.Common.Interface.Models.Op;
using OpInfo = Satelight.Protos.Core.OpInfo;
using OpState = Comms.Common.Interface.Models.OpState;
using RequestType = Comms.Common.Interface.Models.RequestType;

namespace Comms.Common.Implementation.Services;

public class ModelMapper
{
    public UpdateBackgroundReply Map(UpdateBackgroundResponse response) => new() { Result = IntMap(response) };
    public UpdateBackgroundRequest Map(UpdateBackgroundBody input) => IntMap<UpdateBackgroundRequest>(input.UpdateFileInfo);
    public UpdateCoverReply   Map(UpdateCoverResponse response)   => new() { Result = IntMap(response) };
    public UpdateCoverRequest Map(UpdateCoverBody input) => IntMap<UpdateCoverRequest>(input.UpdateFileInfo);
    public GetGameCoverRequest Map(GetGameCoversBody input) => new() { Id = input.GameId };
    public GetGameCoversReply     Map(GetGameCoverResponse response)  => IntMap(response);
    public GetGameBackgroundRequest Map(GetGameBackgroundsBody input) => new() { Id = input.GameId };
    public GetGameBackgroundsReply Map(GetGameBackgroundResponse response) => IntMap(response);
    public InstallGameResponse   Map(InstallGameReply response)      => IntMap<InstallGameResponse>(response.Op);
    public InstallGameBody       Map(InstallGameRequest input)       => new() { GameId = input.Id };
    public InstallGameRequest    Map(InstallGameBody input)          => new() { Id = input.GameId };
    public InstallGameReply      Map(InstallGameResponse response)   => new() { Op = IntMap(response.Op) };
    public UninstallGameResponse Map(UninstallGameReply response)    => IntMap<UninstallGameResponse>(response.Op);
    public UninstallGameBody     Map(UninstallGameRequest input)     => new() { GameId = input.Id };
    public UninstallGameRequest  Map(UninstallGameBody input)        => new() { Id = input.GameId };
    public UninstallGameReply    Map(UninstallGameResponse response) => new() { Op = IntMap(response.Op) };
    public RemoveGameResponse    Map(RemoveGameReply response)       => IntMap<RemoveGameResponse>(response.Result);
    public RemoveGameBody        Map(RemoveGameRequest input)        => new();
    public RemoveGameRequest     Map(RemoveGameBody input)           => new() { Id = input.GameId };
    public RemoveGameReply       Map(RemoveGameResponse response)    => new() { Result = IntMap(response) };
    public RepairGameResponse    Map(RepairGameReply response)       => IntMap<RepairGameResponse>(response.Op);
    public RepairGameBody        Map(RepairGameRequest input)        => new() { GameId = input.Id };
    public RepairGameRequest     Map(RepairGameBody input)           => new() { Id = input.GameId };
    public RepairGameReply       Map(RepairGameResponse response)    => new() { Op = IntMap(response.Op) };
    public UpdateGameResponse    Map(UpdateGameReply response)       => IntMap<UpdateGameResponse>(response.Op);
    public UpdateGameBody        Map(UpdateGameRequest input)        => new() { GameId = input.Id };
    public UpdateGameRequest     Map(UpdateGameBody input)           => new() { Id = input.GameId };
    public UpdateGameReply       Map(UpdateGameResponse response)    => new() { Op = IntMap(response.Op) };
    public StopGameResponse      Map(StopGameReply response)         => IntMap<StopGameResponse>(response.Op);
    public StopGameBody          Map(StopGameRequest input)          => new() { GameId = input.Id };
    public StopGameRequest       Map(StopGameBody input)             => new() { Id = input.GameId };
    public StopGameReply         Map(StopGameResponse response)      => new() { Op = IntMap(response.Op) };
    public StartGameResponse     Map(StartGameReply response)        => IntMap<StartGameResponse>(response.Op);
    public StartGameBody         Map(StartGameRequest input)         => new() { GameId = input.Id };
    public StartGameRequest      Map(StartGameBody input)            => new() { Id = input.GameId };
    public StartGameReply        Map(StartGameResponse response)     => new() { Op = IntMap(response.Op) };
    public GetGameResponse       Map(GetGameReply response)          => IntMap<GetGameResponse>(response.Game);
    public GetGameReply          Map(GetGameResponse response)       => new() { Game = IntMap(response.Game) };
    public GetGameBody           Map(GetGameRequest input)           => new();
    public GetGameRequest        Map(GetGameBody input)              => new() { Id = input.GameId };
    public CountGamesRequest     Map(CountGamesBody input)           => new();
    public CountGamesReply       Map(CountGamesResponse input)       => new() { Count = input.Count };
    public StreamGamesBody       Map(StreamGamesRequest input)       => new();
    public StreamGamesRequest    Map(StreamGamesBody input)          => new();
    public StreamGamesResponse   Map(Game game)                      => IntMap<StreamGamesResponse>(game);
    public Game                  Map(StreamGamesResponse response)   => IntMap(response.Game);
    public MoveGameRequest       Map(MoveGameBody input)             => new() { Id = input.GameId };
    public MoveGameBody          Map(MoveGameRequest input)          => new() { GameId = input.Id };
    public MoveGameResponse      Map(MoveGameReply response)         => IntMap<MoveGameResponse>(response.Op);
    public MoveGameReply         Map(MoveGameResponse response)      => new() { Op = IntMap(response.Op) };
    public InitReply           Map(InitializeResponse response)    => new() { DataPath = response.Path, Port = response.Port };
    public InitializeRequest   Map(InitBody initRequest)           => new();
    public InitBody            Map(InitializeRequest initRequest)  => new();
    public GetCacheReply       Map(GetCacheResponse source)        => IntMap(source);
    public GetCacheResponse    Map(GetCacheReply response)         => IntMap(response);
    public GetCacheRequest     Map(GetCacheBody input)             => new();
    public GetCacheBody        Map(GetCacheRequest input)          => new();
    public GetOpRequest        Map(GetOpBody input)                => new() { Id = input.Id.ToGuid() };
    public GetOpBody           Map(GetOpRequest input)             => new() { Id = input.Id.ToByteString() };
    public GetOpResponse       Map(GetOpReply response)            => new() { Op = IntMap(response.Info) };
    public GetOpReply          Map(GetOpResponse response)         => new() { Info = IntMapToOpInfo(response.Op) };
    public GetOpsRequest       Map(ListOpsBody input)              => new();
    public ListOpsBody         Map(GetOpsRequest input)            => new();
    public GetOpsResponse      Map(ListOpsReply response)          => new() { Ops = IntMap(response.Ops, IntMap) };
    public ListOpsReply        Map(GetOpsResponse response)        => IntMap(response);
    public GetPlatformsRequest Map(ListPlatformsBody input)        => new();
    public ListPlatformsReply  Map(GetPlatformsResponse response)  => IntMap<ListPlatformsReply>(response, r => r.Labels);
    public GetTagsRequest      Map(ListTagsBody input)             => new();
    public ListTagsReply       Map(GetTagsResponse response)       => IntMap<ListTagsReply>(response, r => r.Labels);
    public GetFeaturesRequest  Map(ListFeaturesBody input)         => new();
    public ListFeaturesReply   Map(GetFeaturesResponse response)   => IntMap<ListFeaturesReply>(response, r => r.Labels);
    public GetLibrariesRequest Map(ListLibrariesBody input)        => new();
    public ListLibrariesReply  Map(GetLibrariesResponse response)  => IntMap<ListLibrariesReply>(response, r => r.Labels);
    public GetGenresRequest    Map(ListGenresBody input)           => new();
    public ListGenresReply     Map(GetGenresResponse response)     => IntMap<ListGenresReply>(response, r => r.Labels);
    public GetSeriesRequest    Map(ListSeriesBody input)           => new();
    public ListSeriesReply     Map(GetSeriesResponse response)     => IntMap<ListSeriesReply>(response, r => r.Labels);
    public GetCompaniesRequest  Map(ListCompaniesBody input)       => new();
    public ListCompaniesReply   Map(GetCompaniesResponse response) => IntMap<ListCompaniesReply>(response, r => r.Labels);

    #region Helpers

    private static GetCacheResponse IntMap(GetCacheReply response) => new()
    {
        Genres = IntMap(response.Genres),
        Platforms = IntMap(response.Platforms),
        CompletionStatuses = IntMap(response.CompletionStatuses),
        Tags = IntMap(response.Tags),
        Sources = IntMap(response.Sources)
    };

    private static GetCacheReply IntMap(GetCacheResponse source)
    {
        var target = new GetCacheReply();
        IntMap(target.Genres, source.Genres);
        IntMap(target.Platforms, source.Platforms);
        IntMap(target.CompletionStatuses, source.CompletionStatuses);
        IntMap(target.Tags, source.Tags);
        IntMap(target.Sources, source.Sources);
        return target;
    }
    private static ListOpsReply IntMap(GetOpsResponse response)
    {
        ListOpsReply reply = new();
        IntMap(reply.Ops, response.Ops, IntMap);
        return reply;
    }

    private static GetGameCoversReply IntMap(GetGameCoverResponse input)
    {
        GetGameCoversReply output = new();
        output.Covers.AddRange(input.Files);
        return output;
    }

    private static GetGameBackgroundsReply IntMap(GetGameBackgroundResponse input)
    {
        GetGameBackgroundsReply output = new();
        output.Backgrounds.AddRange(input.Files);
        return output;
    }

    private static Common.Interface.Models.Game IntMap(Game game) => new()
    {
        Name = game.Name,
        Description = game.Description,
        SortingName = game.SortingName,
        Series = game.Series,
        Id = new(game.Id),
        Instances = game.HostGames.Select(IntMap).ToArray()
    };

    private static GameInstance IntMap(HostGame instance) => new()
    {
        Installed   = instance.Installed,
        //ReleaseDate = game.ReleaseDate,
        //DateAdded   = game.DateAdded,
        Playing     = instance.Playing,
        Size        = instance.Size,
        PlayActions = instance.PlayActions,
        Id          = new(instance.Id),
        Platforms   = instance.Platforms.ToList()
    };

    private static Game IntMap(Common.Interface.Models.Game source)
    {
        Game target = new()
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            SortingName = source.SortingName,
            HostGames = { Capacity = source.Instances.Count }
        };
        IntMap(target.HostGames, source.Instances, IntMap);
        target.Genres.AddRange(source.Genres);
        target.Series.AddRange(target.Series);
        return target;
    }

    private static HostGame IntMap(GameInstance source)
    {
        HostGame target = new()
        {
            Installed     = source.Installed,
            Playing       = source.Playing,
            Name          = source.Name,
            Description   = source.Description,
            Size          = source.Size        ?? 0,
            ReleaseDate   = source.ReleaseDate ?? 0,
            Id            = source.Id,
            Library       = source.LibraryId,
            LibraryGameId = source.LibraryGameId
        };
        target.PlayActions.AddRange(source.PlayActions);
        target.Platforms.AddRange(source.Platforms);
        target.Developers.AddRange(source.Developers);
        target.Publishers.AddRange(source.Publishers);
        target.Features.AddRange(source.Features);
        //IntMap(target.Tags,   source);
        target.UserGameInfo.Add(new UserGameInfo 
        { 
            Favorite = source.Favorite
        });
        return target;
    }

    private static T IntMap<T>(UpdateFileInfo info) where T : UpdateGameFileRequest, new() => new()
    {
        Id       = info.Game,
        FileName = info.FileName,
        NewPath  = info.NewPath
    };

    private static TResp IntMap<TResp>(Game game) where TResp : GameResponse, new() => new() { Game = IntMap(game) };

    protected static TOut IntMap<TOut>(
        GetLabelsResponse response, Func<TOut, RepeatedField<Satelight.Protos.Core.Label>> labelSelector)
        where TOut : new()
        => IntMap(response.Items, labelSelector, IntMap);

    protected static TOut IntMap<TIn, TItems, TOut>(
        IList<TIn> inputs,
        Func<TOut, RepeatedField<TItems>> itemSelector,
        Action<RepeatedField<TItems>, IList<TIn>> mapperFunc)
        where TOut : new()
    {
        var target = new TOut();
        mapperFunc(itemSelector(target), inputs);
        return target;
    }

    protected static OpState IntMap(Satelight.Protos.Core.OpState state) => state switch
    {
        Satelight.Protos.Core.OpState.Queued => OpState.Queued,
        Satelight.Protos.Core.OpState.Running => OpState.Running,
        Satelight.Protos.Core.OpState.Paused => OpState.Paused,
        Satelight.Protos.Core.OpState.Finished => OpState.Finished,
        Satelight.Protos.Core.OpState.Failed => OpState.Failed,
        _ => OpState.Unknown,
    };

    protected static Satelight.Protos.Core.OpState IntMap(OpState state) => state switch
    {
        OpState.Queued => Satelight.Protos.Core.OpState.Queued,
        OpState.Running => Satelight.Protos.Core.OpState.Running,
        OpState.Paused => Satelight.Protos.Core.OpState.Paused,
        OpState.Finished => Satelight.Protos.Core.OpState.Finished,
        OpState.Failed => Satelight.Protos.Core.OpState.Failed,
        _ => Satelight.Protos.Core.OpState.Unknown,
    };

    protected static RequestType IntMap(Piped.RequestType type) => type switch
    {
        Piped.RequestType.GetOp => RequestType.GetOp,
        Piped.RequestType.ListTags => RequestType.GetTags,
        Piped.RequestType.GetGame => RequestType.GetGame,
        Piped.RequestType.StreamGames => RequestType.StreamGames,
        Piped.RequestType.GetCache => RequestType.GetCache,
        Piped.RequestType.ListPlatforms => RequestType.GetPlatforms,
        Piped.RequestType.ListGenres => RequestType.GetGenres,
        Piped.RequestType.ListLibraries => RequestType.GetLibraries,
        Piped.RequestType.ListOps => RequestType.GetOps,
        Piped.RequestType.StartGame => RequestType.StartGame,
        Piped.RequestType.StopGame => RequestType.StopGame,
        Piped.RequestType.InstallGame => RequestType.InstallGame,
        Piped.RequestType.UninstallGame => RequestType.UninstallGame,
        Piped.RequestType.UpdateGame => RequestType.UpdateGame,
        Piped.RequestType.RemoveGame => RequestType.RemoveGame,
        Piped.RequestType.RepairGame => RequestType.RepairGame,
        Piped.RequestType.MoveGame => RequestType.MoveGame,
        _ => RequestType.None
    };

    protected static Piped.RequestType IntMap(RequestType type) => type switch
    {
        RequestType.GetOp => Piped.RequestType.GetOp,
        RequestType.GetTags => Piped.RequestType.ListTags,
        RequestType.GetGame => Piped.RequestType.GetGame,
        RequestType.StreamGames => Piped.RequestType.StreamGames,
        RequestType.GetCache => Piped.RequestType.GetCache,
        RequestType.GetPlatforms => Piped.RequestType.ListPlatforms,
        RequestType.GetGenres => Piped.RequestType.ListGenres,
        RequestType.GetLibraries => Piped.RequestType.ListLibraries,
        RequestType.GetOps => Piped.RequestType.ListOps,
        RequestType.StartGame => Piped.RequestType.StartGame,
        RequestType.StopGame => Piped.RequestType.StopGame,
        RequestType.InstallGame => Piped.RequestType.InstallGame,
        RequestType.UninstallGame => Piped.RequestType.UninstallGame,
        RequestType.UpdateGame => Piped.RequestType.UpdateGame,
        RequestType.RemoveGame => Piped.RequestType.RemoveGame,
        RequestType.RepairGame => Piped.RequestType.RepairGame,
        RequestType.MoveGame => Piped.RequestType.MoveGame,
        _ => Piped.RequestType.None
    };

    protected static Op IntMap(OpInfo source) => new()
    {
        GameId = source.TargetId,
        State = IntMap(source.State),
        Type = IntMap(source.Type)
    };

    protected static OpInfo IntMapToOpInfo(Op source) => new()
    {
        TargetId = source.GameId,
        State = IntMap(source.State),
        Type = ToOpType(source.Type)
    };

    protected static Op IntMap(Satelight.Protos.Core.Op op) => new()
    {
        //GameId = new(source.TargetId),
        GameId = null,
        State = IntMap(op.Info.State),
        Type = IntMap(op.Info.Type)
    };

    protected static RequestType IntMap(OpType opType) => opType switch
    {
        OpType.None => RequestType.None,
        OpType.StartGame => RequestType.StartGame,
        OpType.StopGame => RequestType.StopGame,
        OpType.InstallGame => RequestType.InstallGame,
        OpType.UninstallGame => RequestType.UninstallGame,
        OpType.UpdateGame => RequestType.UpdateGame,
        OpType.RemoveGame => RequestType.RemoveGame,
        OpType.RepairGame => RequestType.RepairGame,
        OpType.MoveGame => RequestType.MoveGame,
        OpType.UpdateLibrary => RequestType.UpdateLibrary,
        _ => throw new ArgumentOutOfRangeException(nameof(opType), opType, null)
    };

    protected static OpType ToOpType(RequestType requestType) => requestType switch
    {
        RequestType.None => OpType.None,
        RequestType.StartGame => OpType.StartGame,
        RequestType.StopGame => OpType.StopGame,
        RequestType.InstallGame => OpType.InstallGame,
        RequestType.UninstallGame => OpType.UninstallGame,
        RequestType.UpdateGame => OpType.UpdateGame,
        RequestType.RemoveGame => OpType.RemoveGame,
        RequestType.RepairGame => OpType.RepairGame,
        RequestType.MoveGame => OpType.MoveGame,
        RequestType.UpdateLibrary => OpType.UpdateLibrary,
        _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
    };

    protected static Satelight.Protos.Core.Op IntMap(Op op) => new()
    {
        Id = op.Id.ToByteString(),
        Info = IntMapToOpInfo(op)
    };

    protected static Label IntMap(Satelight.Protos.Core.Label label) => new()
    {
        Id = label.Id,
        Name = label.Name
    };

    protected static Satelight.Protos.Core.Label IntMap(Label label) => new()
    {
        Id = label.Id,
        Name = label.Name
    };

    protected static void IntMap(RepeatedField<Satelight.Protos.Core.Label> target, IList<Label> source)
        => IntMap(target, source, IntMap);

    protected static void IntMap<TTarg, TSour>(RepeatedField<TTarg> target, IList<TSour> source, Func<TSour, TTarg> selector)
        => target.AddRange(source.Select(selector));

    protected static List<Label> IntMap(IList<Satelight.Protos.Core.Label> labels) => IntMap(labels, IntMap);

    protected static List<TOut> IntMap<TIn, TOut>(IList<TIn> bytes, Func<TIn, TOut> selector)
        => [.. bytes.Select(selector)];

    protected static TRes IntMap<TRes>(Satelight.Protos.Core.Op op) where TRes : OpResponse, new()
        => new() { Op = IntMap(op) };

    protected static Result IntMap(SuccessResponse response) => new() { Success = response.Success };
    protected static T IntMap<T>(Result result) where T : SuccessResponse, new()
        => new() { Success = result.Success };

    #endregion
}