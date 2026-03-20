namespace Server.Models.Events;

public sealed record ActiveHostsChangedEventArgs(
    List<Database.Host> Added,
    List<Database.Host> Removed);