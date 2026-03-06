using System;
using System.Collections.Generic;

namespace Comms.Common.Interface.Models;

public class GameInstance
{
    public Guid          Id           { get; set; }
    public string        Name         { get; set; } = string.Empty;
    public string        Description  { get; set; } = string.Empty;
    public bool          Installed    { get; set; }
    public bool          Playing      { get; set; }
    public long?         Size         { get; set; }
    public long?         ReleaseDate  { get; set; }
    public IList<string> PlayActions  { get; set; } = [];
    public Guid          Source       { get; set; }
    public IList<Guid>   Platforms    { get; set; } = [];
    public Guid          PluginId     { get; set; }
    public string        PluginGameId { get; set; } = string.Empty;
    public Guid          LibraryId    { get; set; }
    public Guid[]        Developers   { get; set; } = [];
    public Guid[]        Publishers   { get; set; } = [];
    public Guid[]        Features     { get; set; } = [];
    public bool          Favorite     { get; set; }
}