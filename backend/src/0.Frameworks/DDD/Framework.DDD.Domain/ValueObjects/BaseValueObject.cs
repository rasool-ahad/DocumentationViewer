namespace Framework.DDD.Domain.ValueObjects;

/// <summary>
/// Generic base for value objects with strongly-typed structural equality.
/// Subclasses implement <see cref="GetEqualityComponents"/> returning the parts that define equality.
/// </summary>
/// <typeparam name="TValueObject">The concrete value object type (CRTP).</typeparam>
public abstract class BaseValueObject<TValueObject> : IEquatable<TValueObject>
    where TValueObject : BaseValueObject<TValueObject>
{
    public bool Equals(TValueObject? other) => this == other;

    public override bool Equals(object? obj)
    {
        if (obj is TValueObject otherObject)
            return GetEqualityComponents().SequenceEqual(otherObject.GetEqualityComponents());
        return false;
    }

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);

    /// <summary>Returns the components whose values collectively define equality.</summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public static bool operator ==(BaseValueObject<TValueObject>? left, BaseValueObject<TValueObject>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(BaseValueObject<TValueObject>? left, BaseValueObject<TValueObject>? right) =>
        !(left == right);

    /// <summary>Evaluates a business rule and propagates any exception it throws.</summary>
    protected static void CheckRule(IBusinessRule rule) => rule.IsBroken();
}
