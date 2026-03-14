using Server.Models.Database;
using Server.Models.UserInterface;

namespace Server.Models.Events;

public class GameDisplayUpdatedEventArgs
{
    public GameInfo GameInfo { get; set; }
    public OperationType? Type { get; set; }
    public OperationState? State { get; set; }
}