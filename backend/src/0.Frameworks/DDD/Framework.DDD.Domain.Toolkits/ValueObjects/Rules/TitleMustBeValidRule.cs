using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Toolkits.ValueObjects.Rules;

public class TitleMustBeValidRule : IBusinessRule
{
    private readonly string _value;
    public List<string> Messages { get; set; } = [];

    public TitleMustBeValidRule(string value) => _value = value;

    public void IsBroken()
    {
        if (string.IsNullOrWhiteSpace(_value))
        {
            Messages.Add("Title is required.");
            throw new ArgumentException(string.Join(", ", Messages));
        }
        if (_value.Length < 2 || _value.Length > 250)
        {
            Messages.Add("Title must be between 2 and 250 characters.");
            throw new ArgumentException(string.Join(", ", Messages));
        }
    }
}
