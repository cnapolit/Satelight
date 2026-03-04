using System.Threading;

namespace Common.Utility.Models;

public class TokenConditional(CancellationToken token) : Conditional
{
    public override bool Condition => !token.IsCancellationRequested;
}