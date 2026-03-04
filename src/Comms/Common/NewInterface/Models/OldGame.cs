using System;
using System.Collections.Generic;

namespace Comms.Common.Interface.Models;

public class OldGame
{
    public Guid Id { get; set; }
    public bool Favorite { get; set; }
    public long? ReleaseDate { get; set; }
    public long? LastPlayed { get; set; }
    public long TimePlayed { get; set; }
    public long DateAdded { get; set; }
    public int UserScore { get; set; }
    public int CriticScore { get; set; }
    public int CommunityScore { get; set; }
    public Guid CompletionStatus { get; set; }
    public string Name { get; set; }
    public string Publisher { get; set; }
    public string Description { get; set; }
    public string Notes { get; set; }
    public IList<string> Developers { get; set; } = [];
    public IList<Guid> Tags { get; set; } = [];
    public IList<Guid> Features { get; set; } = [];
    public IList<Guid> Genres { get; set; } = [];
    public IList<DeviceState> DeviceStates { get; set; } = [];
    public string Cover { get; set; }
    public string Background { get; set; }
    public string SortingName { get; set; }
    public long PlayCount { get; set; }
    public IList<string> Series { get; set; } = [];
    public string Icon { get; set; }
    public IList<Guid> Categories { get; set; } = [];
}