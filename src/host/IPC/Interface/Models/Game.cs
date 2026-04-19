
namespace Comms.Common.Interface.Models;

public class Game
{
    public required string     Id               { get; set; }
    public required string     Name             { get; set; }
    public bool                Favorite         { get; set; }
    public long?               LastPlayed       { get; set; }
    public long                TimePlayed       { get; set; }
    public int                 UserScore        { get; set; }
    public int                 CriticScore      { get; set; }
    public int                 CommunityScore   { get; set; }
    public string?             CompletionStatus { get; set; }
    public IList<string>       Publishers       { get; set; } = [];
    public string?             Description      { get; set; }
    public string?             Notes            { get; set; }
    public IList<string>       Developers       { get; set; } = [];
    public IList<string>       Tags             { get; set; } = [];
    public IList<string>       Features         { get; set; } = [];
    public IList<string>       Genres           { get; set; } = [];
    public IList<GameInstance> Instances        { get; set; } = [];
    public string?             Cover            { get; set; }
    public string?             Background       { get; set; }
    public string?             SortingName      { get; set; }
    public long                PlayCount        { get; set; }
    public IList<string>       Series           { get; set; } = [];
    public string?             Icon             { get; set; }
    public IList<string>       Categories       { get; set; } = [];
}