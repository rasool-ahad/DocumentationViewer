using Framework.DDD.Domain.Exceptions;
using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Toolkits.ValueObjects;

/// <summary>
/// A strongly-typed, globally unique identifier for a domain entity.
/// Used as the business-facing identity (as opposed to a database surrogate key).
/// </summary>
public class BusinessId : BaseValueObject<BusinessId>
{
    public Guid Value { get; private set; }

    public static BusinessId FromString(string value) => new(value);
    public static BusinessId FromGuid(Guid value) => new() { Value = value };

    public BusinessId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidValueObjectStateException("ValidationErrorIsRequire", nameof(BusinessId));

        if (Guid.TryParse(value, out Guid parsed))
            Value = parsed;
        else
            throw new InvalidValueObjectStateException("ValidationErrorInvalidValue", nameof(BusinessId));
    }

    private BusinessId() { }

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator string(BusinessId id) => id.Value.ToString();
    public static implicit operator BusinessId(string value) => new(value);

    public static explicit operator Guid(BusinessId id) => id.Value;
    public static implicit operator BusinessId(Guid value) => new() { Value = value };
}
