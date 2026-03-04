using Microsoft.EntityFrameworkCore;

namespace Server.Models.Database;

[PrimaryKey(nameof(UserId), nameof(GameId))]
public class UserGameInfo
{
    public Guid      UserId             { get; set; }
    public Guid      GameId             { get; set; }
    public short?    Score              { get; set; }
    public bool      Favorite           { get; set; }
    public DateTime? LastActivity       { get; set; }
    public long?     PlayCount          { get; set; }
    public long?     PlayTime           { get; set; }
    public Guid      CompletionStatusId { get; set; }

    public User                         User             { get; set; }
    public Game                         Game             { get; set; }
    public CompletionStatus             CompletionStatus { get; set; }
    public ICollection<UserGameSession> UserGameSessions { get; set; } = [];
    public ICollection<Category>        Categories       { get; set; } = [];
}