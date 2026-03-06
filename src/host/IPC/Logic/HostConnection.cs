using Comms.Common.Implementation;
using Comms.Common.Interface.Models;
using Comms.Host.Interface;
using ProtoBuf;
using Satelight.Protos.Host;
using System.IO;
using System.IO.Pipes;
using Comms.Host.Implementation.Services;
using Comms.Host.Interface.Models;
using Piped;
using RequestType = Piped.RequestType;

namespace Comms.Host.Implementation;

public class HostConnection(NamedPipeServerStream stream) : SatelightConnection(stream), IHostConnection
{
    private readonly HostModelMapper  _mapper = new();

    protected override SatelightRequest Map(RequestType type, MemoryStream paramStream) => type switch
    {
        RequestType.GetGameCover      => _mapper.Map(Serializer.Deserialize     <GetGameCoversBody>(paramStream)),
        RequestType.GetGameBackground => _mapper.Map(Serializer.Deserialize<GetGameBackgroundsBody>(paramStream)),
        RequestType.GetGame           => _mapper.Map(Serializer.Deserialize           <GetGameBody>(paramStream)),
        RequestType.StreamGames       => _mapper.Map(Serializer.Deserialize       <StreamGamesBody>(paramStream)),
        RequestType.CountGames        => _mapper.Map(Serializer.Deserialize        <CountGamesBody>(paramStream)),
        RequestType.StartGame         => _mapper.Map(Serializer.Deserialize         <StartGameBody>(paramStream)),
        RequestType.StopGame          => _mapper.Map(Serializer.Deserialize          <StopGameBody>(paramStream)),
        RequestType.InstallGame       => _mapper.Map(Serializer.Deserialize       <InstallGameBody>(paramStream)),
        RequestType.UninstallGame     => _mapper.Map(Serializer.Deserialize     <UninstallGameBody>(paramStream)),
        RequestType.UpdateGame        => _mapper.Map(Serializer.Deserialize        <UpdateGameBody>(paramStream)),
        RequestType.RemoveGame        => _mapper.Map(Serializer.Deserialize        <RemoveGameBody>(paramStream)),
        RequestType.RepairGame        => _mapper.Map(Serializer.Deserialize        <RepairGameBody>(paramStream)),
        RequestType.MoveGame          => _mapper.Map(Serializer.Deserialize          <MoveGameBody>(paramStream)),
        RequestType.UpdateGameCover   => _mapper.Map(Serializer.Deserialize       <UpdateCoverBody>(paramStream)),
        RequestType.UpdateGameBackground => _mapper.Map(Serializer.Deserialize<UpdateBackgroundBody>(paramStream)),
        _                             =>   base.Map(type, paramStream)
    };

    protected override object Map<T>(T response) => response switch
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
        UpdateBackgroundResponse   updateBackgroundResponse => _mapper.Map(updateBackgroundResponse),
        _                                                   =>   base.Map                 (response)
    };
}