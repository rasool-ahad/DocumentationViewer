using Framework.DDD.Domain.Toolkits.ValueObjects.Rules;
using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Toolkits.ValueObjects;

public class Title : BaseValueObject<Title>
{
    #region Properties
    public string Value { get; private set; } = default!;
    #endregion

    #region Constructors and Factories
    public static Title FromString(string value) => new(value);

    private Title(string value)
    {
        CheckRule(new TitleMustBeValidRule(value));
        Value = value;
    }

    private Title() { }
    #endregion

    #region Equality
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    #endregion

    #region Operators
    public static explicit operator string(Title title) => title.Value;
    public static implicit operator Title(string value) => new(value);
    #endregion

    public override string ToString() => Value;
}
