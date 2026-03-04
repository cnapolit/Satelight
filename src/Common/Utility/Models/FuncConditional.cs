using System;

namespace Common.Utility.Models;

public class FuncConditional(Func<bool> conditionFunc) : Conditional
{
    public override bool Condition => conditionFunc();
}