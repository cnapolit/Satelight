namespace Common.Utility.Models;

public abstract class Conditional
{
    public static implicit operator bool(Conditional d) => d.Condition;
    public abstract bool Condition { get; }
}