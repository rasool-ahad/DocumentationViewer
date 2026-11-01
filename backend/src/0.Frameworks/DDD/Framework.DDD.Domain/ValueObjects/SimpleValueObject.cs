namespace Framework.DDD.Domain.ValueObjects;

/// <summary>
/// Value object that wraps a single comparable value of type <typeparamref name="T"/>.
/// Suitable for strongly-typed primitives such as Money, Percentage, EmailAddress, etc.
/// </summary>
[Serializable]
public abstract class SimpleValueObject<T> : ValueObject
    where T : IComparable
{
    /// <summary>The underlying value.</summary>
    public T Value { get; }

    protected SimpleValueObject(T value)
    {
        Value = value;
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string? ToString() => Value?.ToString();

    /// <summary>Implicitly unwraps the value object to its underlying value.</summary>
    public static implicit operator T(SimpleValueObject<T> valueObject) =>
        valueObject is null ? default! : valueObject.Value;
}
