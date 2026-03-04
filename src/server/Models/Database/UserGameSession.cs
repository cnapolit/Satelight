namespace Server.Models.Database;

public class UserGameSession : DatabaseObject
{
    public DateTime StartTime { get; set; }
    public long     Duration  { get; set; }

    public UserGameInfo UserGameInfo { get; set; }
    public LibraryGame  LibraryGame  { get; set; }
}