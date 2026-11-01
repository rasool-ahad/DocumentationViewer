using Framework.DDD.Domain.Toolkits.ValueObjects.Rules;
using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Toolkits.ValueObjects;

public class Description : BaseValueObject<Description>
{
    #region Properties
    public string? Value { get; private set; }
    #endregion

    #region Constructors and Factories
    public static Description FromString(string? value) => new(value);

    private Description(string? value)
    {
        CheckRule(new DescriptionMustBeValidRule(value));
        Value = value;
    }

    private Description() { }
    #endregion

    #region Equality
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? string.Empty;
    }
    #endregion

    #region Operators
    public static explicit operator string?(Description description) => description.Value;
    public static implicit operator Description(string? value) => new(value);
    #endregion

    public override string? ToString() => Value;
}
