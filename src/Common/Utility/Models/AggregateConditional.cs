using Common.Utility.Extensions;

namespace Common.Utility.Models;

public class AggregateConditional(params Conditional[] conditionals) : Conditional
{
    public override bool Condition => conditionals.Length > 0 && conditionals.All();
}