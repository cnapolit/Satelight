namespace Server.Models.Events;

public sealed record GameUpdatedEventArgs(
    Guid GameId);
