namespace Framework.DDD.Domain.ValueObjects;

/// <summary>
/// Generic value object base for strongly-typed equality via custom <see cref="EqualsCore"/>
/// and <see cref="GetHashCodeCore"/> implementations.
/// </summary>
[Serializable]
public abstract class ValueObject<T> where T : ValueObject<T>
{
    private int? _cachedHashCode;

    public override bool Equals(object? obj)
    {
        if (obj is not T valueObject) return false;
        if (ValueObject.GetUnproxiedType(this) != ValueObject.GetUnproxiedType(obj)) return false;
        return EqualsCore(valueObject);
    }

    /// <summary>Implement to define equality between two instances of <typeparamref name="T"/>.</summary>
    protected abstract bool EqualsCore(T other);

    public override int GetHashCode()
    {
        if (!_cachedHashCode.HasValue)
            _cachedHashCode = GetHashCodeCore();
        return _cachedHashCode.Value;
    }

    /// <summary>Implement to provide the hash code for this value object.</summary>
    protected abstract int GetHashCodeCore();

    public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(ValueObject<T> a, ValueObject<T> b) => !(a == b);
}
