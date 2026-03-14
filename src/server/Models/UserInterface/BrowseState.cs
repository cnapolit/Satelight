namespace Server.Models.UserInterface;

public class BrowseState
{
    public event Action? SelectedGameChanged;
    public GameDisplayInfo? SelectedGame
    {
        get;
        set
        {
            field = value;
            SelectedGameChanged?.Invoke();
        }
    }
    public event Action? GameInfoChanged;
    public GameInfo? GameInfo
    { 
        get;
        set
        {
            field = value;
            GameInfoChanged?.Invoke();
        }
    }

    public List<GameDisplayInfo> Games { get; set; } = [];
    public GameListState GameListState { get; set; }
    public string? PlayingGameId { get; set; }
}