using Comms.Common.Interface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Piped;
using Satelight.Protos.Core;
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
    public InitReply           Map(InitializeResponse response)    => new() { DataPath = response.Path, Port = response.Port };
    public InitializeRequest   Map(InitBody initRequest)        => new();
    public InitBody Map(InitializeRequest initRequest)  => new();
    public GetCacheReply       Map(GetCacheResponse source)        => IntMap(source);
    public GetCacheResponse    Map(GetCacheReply response)         => IntMap(response);
    public GetCacheRequest     Map(GetCacheBody input)             => new();
    public GetCacheBody        Map(GetCacheRequest input)          => new();
    public GetOpRequest        Map(GetOpBody input)                => new() { Id = new(input.Id) };
    public GetOpBody           Map(GetOpRequest input)             => new() { Id = input.Id.ToByteArray() };
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

    #region Internal

    private static GetCacheResponse IntMap(GetCacheReply response) => new()
    {
        Genres             = IntMap(response.Genres),
        Platforms          = IntMap(response.Platforms),
        CompletionStatuses = IntMap(response.CompletionStatuses),
        Tags               = IntMap(response.Tags),
        Sources            = IntMap(response.Sources)
    };

    private static GetCacheReply IntMap(GetCacheResponse source)
    {
        var target = new GetCacheReply();
        IntMap(target.Genres,             source.Genres);
        IntMap(target.Platforms,          source.Platforms);
        IntMap(target.CompletionStatuses, source.CompletionStatuses);
        IntMap(target.Tags,               source.Tags);
        IntMap(target.Sources,            source.Sources);
        return target;
    }
    private static ListOpsReply IntMap(GetOpsResponse response)
    {
        ListOpsReply reply = new();
        IntMap(reply.Ops, response.Ops, IntMap);
        return reply;
    }

    #endregion

    #region Helpers

    protected static TOut IntMap<TOut>(
        GetLabelsResponse response, Func<TOut, List<Satelight.Protos.Core.Label>> labelSelector)
        where TOut : new() 
        => IntMap(response.Items, labelSelector, IntMap);

    protected static TOut IntMap<TIn, TItems, TOut>(
        IList<TIn> inputs,
        Func<TOut, List<TItems>> itemSelector,
        Action<List<TItems>, IList<TIn>> mapperFunc)
        where TOut : new()
    {
        var target = new TOut();
        mapperFunc(itemSelector(target), inputs);
        return target;
    }

    protected static OpState IntMap(Satelight.Protos.Core.OpState state) => state switch
    {
        Satelight.Protos.Core.OpState.Queued   => OpState.Queued,
        Satelight.Protos.Core.OpState.Running  => OpState.Running,
        Satelight.Protos.Core.OpState.Paused   => OpState.Paused,
        Satelight.Protos.Core.OpState.Finished => OpState.Finished,
        Satelight.Protos.Core.OpState.Failed   => OpState.Failed,
        _                                      => OpState.Unknown,
    };

    protected static Satelight.Protos.Core.OpState IntMap(OpState state) => state switch
    {
        OpState.Queued   => Satelight.Protos.Core.OpState.Queued,
        OpState.Running  => Satelight.Protos.Core.OpState.Running,
        OpState.Paused   => Satelight.Protos.Core.OpState.Paused,
        OpState.Finished => Satelight.Protos.Core.OpState.Finished,
        OpState.Failed   => Satelight.Protos.Core.OpState.Failed,
        _                => Satelight.Protos.Core.OpState.Unknown,
    };

    protected static RequestType IntMap(Piped.RequestType type) => type switch
    {
        Piped.RequestType.GetOp         => RequestType.GetOp,
        Piped.RequestType.ListTags      => RequestType.GetTags,
        Piped.RequestType.GetGame       => RequestType.GetGame,
        Piped.RequestType.StreamGames   => RequestType.StreamGames,
        Piped.RequestType.GetCache      => RequestType.GetCache,
        Piped.RequestType.ListPlatforms => RequestType.GetPlatforms,
        Piped.RequestType.ListGenres    => RequestType.GetGenres,
        Piped.RequestType.ListLibraries => RequestType.GetLibraries,
        Piped.RequestType.ListOps       => RequestType.GetOps,
        Piped.RequestType.StartGame     => RequestType.StartGame,
        Piped.RequestType.StopGame      => RequestType.StopGame,
        Piped.RequestType.InstallGame   => RequestType.InstallGame,
        Piped.RequestType.UninstallGame => RequestType.UninstallGame,
        Piped.RequestType.UpdateGame    => RequestType.UpdateGame,
        Piped.RequestType.RemoveGame    => RequestType.RemoveGame,
        Piped.RequestType.RepairGame    => RequestType.RepairGame,
        Piped.RequestType.MoveGame      => RequestType.MoveGame,
        _                               => RequestType.None
    };

    protected static Piped.RequestType IntMap(RequestType type) => type switch
    {
        RequestType.GetOp         => Piped.RequestType.GetOp,
        RequestType.GetTags       => Piped.RequestType.ListTags,
        RequestType.GetGame       => Piped.RequestType.GetGame,
        RequestType.StreamGames   => Piped.RequestType.StreamGames,
        RequestType.GetCache      => Piped.RequestType.GetCache,
        RequestType.GetPlatforms  => Piped.RequestType.ListPlatforms,
        RequestType.GetGenres     => Piped.RequestType.ListGenres,
        RequestType.GetLibraries  => Piped.RequestType.ListLibraries,
        RequestType.GetOps        => Piped.RequestType.ListOps,
        RequestType.StartGame     => Piped.RequestType.StartGame,
        RequestType.StopGame      => Piped.RequestType.StopGame,
        RequestType.InstallGame   => Piped.RequestType.InstallGame,
        RequestType.UninstallGame => Piped.RequestType.UninstallGame,
        RequestType.UpdateGame    => Piped.RequestType.UpdateGame,
        RequestType.RemoveGame    => Piped.RequestType.RemoveGame,
        RequestType.RepairGame    => Piped.RequestType.RepairGame,
        RequestType.MoveGame      => Piped.RequestType.MoveGame,
        _                         => Piped.RequestType.None
    };

    protected static Op IntMap(OpInfo source) => new()
    {
        GameId = new(source.TargetId),
        State  = IntMap(source.State),
        Type   = IntMap(source.Type)
    };

    protected static OpInfo IntMapToOpInfo(Op source) => new()
    {
        TargetId = source.Id.ToByteArray(),
        State    = IntMap(source.State),
        Type     = ToOpType(source.Type)
    };

    protected static Op IntMap(Satelight.Protos.Core.Op op) => new()
    {
        GameId = new(op.Id),
        Id     = new(op.Id),
        State  = IntMap(op.Info.State),
        Type   = IntMap(op.Info.Type)
    };

    protected static RequestType IntMap(OpType opType) => opType switch
    {
        OpType.None          => RequestType.None,
        OpType.StartGame     => RequestType.StartGame,
        OpType.MonitorGame   => RequestType.MonitorGame,
        OpType.StopGame      => RequestType.StopGame,
        OpType.InstallGame   => RequestType.InstallGame,
        OpType.UninstallGame => RequestType.UninstallGame,
        OpType.UpdateGame    => RequestType.UpdateGame,
        OpType.RemoveGame    => RequestType.RemoveGame,
        OpType.RepairGame    => RequestType.RepairGame,
        OpType.MoveGame      => RequestType.MoveGame,
        OpType.UpdateLibrary => RequestType.UpdateLibrary,
        _                    => throw new ArgumentOutOfRangeException(nameof(opType), opType, null)
    };

    protected static OpType ToOpType(RequestType requestType) => requestType switch
    {
        RequestType.None          => OpType.None,
        RequestType.StartGame     => OpType.StartGame,
        RequestType.MonitorGame   => OpType.MonitorGame,
        RequestType.StopGame      => OpType.StopGame,
        RequestType.InstallGame   => OpType.InstallGame,
        RequestType.UninstallGame => OpType.UninstallGame,
        RequestType.UpdateGame    => OpType.UpdateGame,
        RequestType.RemoveGame    => OpType.RemoveGame,
        RequestType.RepairGame    => OpType.RepairGame,
        RequestType.MoveGame      => OpType.MoveGame,
        RequestType.UpdateLibrary => OpType.UpdateLibrary,
        _                         => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
    };



    protected static Satelight.Protos.Core.Op IntMap(Op op) => new()
    {
        Id   = op.Id.ToByteArray(),
        Info = IntMapToOpInfo(op)
    };

    protected static Label IntMap(Satelight.Protos.Core.Label label) => new()
    {
        Id   = new(label.Id),
        Name = label.Name
    };

    protected static Satelight.Protos.Core.Label IntMap(Label label) => new()
    {
        Id   = label.Id.ToByteArray(),
        Name = label.Name
    };

    protected static void IntMap(List<byte[]> target, IList<Guid> source)
        => IntMap(target, source, IntMap);

    protected static void IntMap(List<Satelight.Protos.Core.Label> target, IList<Label> source)
        => IntMap(target, source, IntMap);

    protected static void IntMap<TTarg, TSour>(List<TTarg> target, IList<TSour> source, Func<TSour, TTarg> selector)
        => target.AddRange(source.Select(selector));

    protected static List<Label>   IntMap(IList<Satelight.Protos.Core.Label> labels) => IntMap(labels, IntMap);
    protected static IList<Guid>   IntMap(List<byte[]> bytes)                        => IntMap(bytes,  IntMap);
    protected static IList<byte[]> IntMap(List<Guid> guids)                          => IntMap(guids,  IntMap);
    protected static Guid          IntMap(byte[] bytes)                              => new(bytes);
    protected static byte[]        IntMap(Guid guid)                                 => guid.ToByteArray();

    protected static List<TOut> IntMap<TIn, TOut>(IList<TIn> bytes, Func<TIn, TOut> selector)
        => [..bytes.Select(selector)];

    protected static TRes IntMap<TRes>(Satelight.Protos.Core.Op op) where TRes : OpResponse, new()
        => new() { Op = IntMap(op) };

    protected static Result IntMap(SuccessResponse response) => new() { Success = response.Success };
    protected static T IntMap<T>(Result result) where T : SuccessResponse, new()
        => new() { Success = result.Success };


    #endregion
}