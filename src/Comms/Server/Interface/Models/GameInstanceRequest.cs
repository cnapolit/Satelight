using System;
using Comms.Common.Interface.Models;

namespace Comms.Server.Interface.Models;

public class GameInstanceRequest : SatelightRequest
{
    public Guid GameId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid DeviceGameId { get; set; }

    public Guid LibraryId { get; set; }
    public string LibraryGameId { get; set; }
}