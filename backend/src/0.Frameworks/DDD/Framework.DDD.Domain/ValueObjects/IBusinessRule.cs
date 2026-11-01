namespace Framework.DDD.Domain.ValueObjects;

/// <summary>
/// Represents a business rule that can be evaluated against domain objects.
/// </summary>
public interface IBusinessRule
{
    /// <summary>Evaluates the rule; should throw when the rule is broken.</summary>
    void IsBroken();

    /// <summary>Human-readable messages describing why the rule is broken.</summary>
    List<string> Messages { get; set; }
}
