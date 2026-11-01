namespace Framework.DDD.Domain.ValueObjects;

/// <summary>
/// Abstract base for value objects that support structural equality and ordering.
/// Subclasses implement <see cref="GetEqualityComponents"/> to define what makes two instances equal.
/// Also exposes the <see cref="GetUnproxiedType"/> helper used by entity equality to work around EF Core proxies.
/// </summary>
[Serializable]
public abstract class ValueObject : IComparable, IComparable<ValueObject>
{
    private int? _cachedHashCode;

    /// <summary>
    /// Returns the components whose values collectively define equality for this value object.
    /// </summary>
    protected abstract IEnumerable<IComparable> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (GetUnproxiedType(this) != GetUnproxiedType(obj)) return false;
        return GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        if (!_cachedHashCode.HasValue)
        {
            _cachedHashCode = GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked { return current * 23 + (obj?.GetHashCode() ?? 0); }
                });
        }
        return _cachedHashCode.Value;
    }

    public virtual int CompareTo(ValueObject? other)
    {
        if (other is null) return 1;
        if (ReferenceEquals(this, other)) return 0;

        var thisType = GetUnproxiedType(this);
        var otherType = GetUnproxiedType(other);

        if (thisType != otherType)
            return string.Compare($"{thisType}", $"{otherType}", StringComparison.Ordinal);

        return GetEqualityComponents()
            .Zip(other.GetEqualityComponents(), (left, right) =>
                left?.CompareTo(right) ?? (right is null ? 0 : -1))
            .FirstOrDefault(cmp => cmp != 0);
    }

    public virtual int CompareTo(object? other) => CompareTo(other as ValueObject);

    public static bool operator ==(ValueObject a, ValueObject b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.Equals(b);
    }

    public static bool operator !=(ValueObject a, ValueObject b) => !(a == b);

    /// <summary>
    /// Returns the real CLR type of <paramref name="obj"/>, unwrapping EF Core Castle proxy types.
    /// Used internally by entity and value object equality comparisons.
    /// </summary>
    internal static Type GetUnproxiedType(object obj)
    {
        const string efCoreProxyPrefix = "Castle.Proxies.";
        const string proxyPostfix = "Proxy";

        Type type = obj.GetType();
        string typeString = type.ToString();

        if (typeString.Contains(efCoreProxyPrefix) || typeString.EndsWith(proxyPostfix))
            return type.BaseType!;

        return type;
    }
}
