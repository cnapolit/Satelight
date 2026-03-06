using Comms.Common.Interface.Models;
using Comms.Host.Interface.Models;
using System.Linq;
using Comms.Common.Implementation.Services;
using Piped;
using Satelight.Protos.Host;
using Game = Satelight.Protos.Host.Game;

namespace Comms.Host.Implementation.Services;

public class HostModelMapper : ModelMapper
{
    public UpdateBackgroundReply Map(UpdateBackgroundResponse response) => new() { Result = IntMap(response) };
    public UpdateBackgroundRequest Map(UpdateBackgroundBody input) => IntMap<UpdateBackgroundRequest>(input.UpdateFileInfo);
    public UpdateCoverReply   Map(UpdateCoverResponse response)   => new() { Result = IntMap(response) };
    public UpdateCoverRequest Map(UpdateCoverBody input) => IntMap<UpdateCoverRequest>(input.UpdateFileInfo);
    public GetGameCoverRequest Map(GetGameCoversBody input) => new() { Id = IntMap(input.Id) };
    public GetGameCoversReply     Map(GetGameCoverResponse response)  => IntMap(response);
    public GetGameBackgroundRequest Map(GetGameBackgroundsBody input) => new() { Id = IntMap(input.Id) };
    public GetGameBackgroundsReply Map(GetGameBackgroundResponse response) => IntMap(response);
    public InstallGameResponse   Map(InstallGameReply response)      => IntMap<InstallGameResponse>(response.Op);
    public InstallGameBody       Map(InstallGameRequest input)       => new() { GameIdentifier = IntMap(input) };
    public InstallGameRequest    Map(InstallGameBody input)          => new() { Id = IntMap(input.GameIdentifier.Id) };
    public InstallGameReply      Map(InstallGameResponse response)   => new() { Op = IntMap(response.Op) };
    public UninstallGameResponse Map(UninstallGameReply response)    => IntMap<UninstallGameResponse>(response.Op);
    public UninstallGameBody     Map(UninstallGameRequest input)     => new() { GameIdentifier = IntMap(input) };
    public UninstallGameRequest  Map(UninstallGameBody input)        => new() { Id = IntMap(input.GameIdentifier.Id) };
    public UninstallGameReply    Map(UninstallGameResponse response) => new() { Op = IntMap(response.Op) };
    public RemoveGameResponse    Map(RemoveGameReply response)       => IntMap<RemoveGameResponse>(response.Result);
    public RemoveGameBody        Map(RemoveGameRequest input)        => new();
    public RemoveGameRequest     Map(RemoveGameBody input)           => new() { Id = IntMap(input.GameId) };
    public RemoveGameReply       Map(RemoveGameResponse response)    => new() { Result = IntMap(response) };
    public RepairGameResponse    Map(RepairGameReply response)       => IntMap<RepairGameResponse>(response.Op);
    public RepairGameBody        Map(RepairGameRequest input)        => new() { GameIdentifier = IntMap(input) };
    public RepairGameRequest     Map(RepairGameBody input)           => new() { Id             = IntMap(input.GameIdentifier.Id) };
    public RepairGameReply       Map(RepairGameResponse response)    => new() { Op             = IntMap(response.Op) };
    public UpdateGameResponse    Map(UpdateGameReply response)       => IntMap<UpdateGameResponse>(response.Op);
    public UpdateGameBody        Map(UpdateGameRequest input)        => new() { GameIdentifier = IntMap(input) };
    public UpdateGameRequest     Map(UpdateGameBody input)           => new() { Id             = IntMap(input.GameIdentifier.Id) };
    public UpdateGameReply       Map(UpdateGameResponse response)    => new() { Op             = IntMap(response.Op) };
    public StopGameResponse      Map(StopGameReply response)         => IntMap<StopGameResponse>(response.Op);
    public StopGameBody          Map(StopGameRequest input)          => new() { GameIdentifier = IntMap(input) };
    public StopGameRequest       Map(StopGameBody input)             => new() { Id = IntMap(input.GameIdentifier.Id) };
    public StopGameReply         Map(StopGameResponse response)      => new() { Op = IntMap(response.Op) };
    public StartGameResponse     Map(StartGameReply response)        => IntMap<StartGameResponse>(response.Op);
    public StartGameBody         Map(StartGameRequest input)         => new() { GameIdentifier = IntMap(input) };
    public StartGameRequest      Map(StartGameBody input)            => new() { Id             = IntMap(input.GameIdentifier.Id) };
    public StartGameReply        Map(StartGameResponse response)     => new() { Op             = IntMap(response.Op) };
    public GetGameResponse       Map(GetGameReply response)          => IntMap<GetGameResponse>(response.Game);
    public GetGameReply          Map(GetGameResponse response)       => new() { Game = IntMap(response.Game) };
    public GetGameBody           Map(GetGameRequest input)           => new();
    public GetGameRequest        Map(GetGameBody input)              => new() { Id = IntMap(input.GameId) };
    public CountGamesRequest     Map(CountGamesBody input)           => new();
    public CountGamesReply       Map(CountGamesResponse input)       => new() { Count = input.Count };
    public StreamGamesBody       Map(StreamGamesRequest input)       => new();
    public StreamGamesRequest    Map(StreamGamesBody input)          => new();
    public StreamGamesResponse   Map(Game game)                      => IntMap<StreamGamesResponse>(game);
    public Game                  Map(StreamGamesResponse response)   => IntMap(response.Game);
    public MoveGameRequest       Map(MoveGameBody input)             => new() { Id             = IntMap(input.GameIdentifier.Id) };
    public MoveGameBody          Map(MoveGameRequest input)          => new() { GameIdentifier = IntMap(input) };
    public MoveGameResponse      Map(MoveGameReply response)         => IntMap<MoveGameResponse>(response.Op);
    public MoveGameReply         Map(MoveGameResponse response)      => new() { Op = IntMap(response.Op) };

    #region Helpers

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
        Platforms   = IntMap(instance.Platforms)
    };

    //private static Op IntMap(Core.OpInfo info) => new()
    //{
    //    GameId = new(info.TargetId),
    //    State  = IntMap(info.State),
    //    Type   = IntMap(info.Type)
    //};


    private static Game IntMap(Common.Interface.Models.Game source)
    {
        Game target = new()
        {
            Id = source.Id.ToByteArray(),
      
            Name = source.Name,
            Description = source.Description,
            SortingName = source.SortingName,
            HostGames = { Capacity = source.Instances.Count }
        };
        IntMap(target.HostGames, source.Instances, IntMap);
        IntMap(target.Genres,    source.Genres);
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
            Id            = source.Id.ToByteArray(),
            Library       = source.LibraryId.ToByteArray(),
            LibraryGameId = source.PluginGameId
        };
        target.PlayActions.AddRange(source.PlayActions);
        IntMap(target.Platforms,  source.Platforms);
        IntMap(target.Developers, source.Developers);
        IntMap(target.Publishers, source.Publishers);
        IntMap(target.Features,   source.Features);
        //IntMap(target.Tags,   source);
        target.UserGameInfoes.Add(new UserGameInfo 
        { 
            Favorite = source.Favorite
        });
        return target;
    }

    private static T IntMap<T>(UpdateFileInfo info) where T : UpdateGameFileRequest, new() => new()
    {
        Id       = IntMap(info.Game),
        FileName = info.fileName,
        NewPath  = info.NewPath
    };

    private static GameIdentifier IntMap(GameRequest request) => new() { Id = request.Id.ToByteArray() };
    private static TResp IntMap<TResp>(Game game) where TResp : GameResponse, new() => new() { Game = IntMap(game) };

    #endregion
}