using Framework.DDD.Domain.Toolkits.ValueObjects.Rules;
using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Toolkits.ValueObjects;

public class IranianPhoneNumber : BaseValueObject<IranianPhoneNumber>
{
    #region Properties
    public string Value { get; private set; } = default!;
    #endregion

    #region Constructors and Factories
    public static IranianPhoneNumber FromString(string value) => new(value);

    private IranianPhoneNumber(string value)
    {
        CheckRule(new IranianPhoneNumberMustBeValidRule(value));
        Value = value;
    }

    private IranianPhoneNumber() { }
    #endregion

    #region Equality
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    #endregion

    #region Operators
    public static explicit operator string(IranianPhoneNumber phone) => phone.Value;
    public static implicit operator IranianPhoneNumber(string value) => new(value);
    #endregion

    public override string ToString() => Value;
}
