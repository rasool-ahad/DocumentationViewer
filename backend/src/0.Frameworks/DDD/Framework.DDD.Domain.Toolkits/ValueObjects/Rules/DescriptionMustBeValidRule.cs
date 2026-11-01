using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Toolkits.ValueObjects.Rules;

public class DescriptionMustBeValidRule : IBusinessRule
{
    private readonly string? _value;
    public List<string> Messages { get; set; } = [];

    public DescriptionMustBeValidRule(string? value) => _value = value;

    public void IsBroken()
    {
        if (!string.IsNullOrWhiteSpace(_value) && _value.Length > 500)
        {
            Messages.Add("Description must not exceed 500 characters.");
            throw new ArgumentException(string.Join(", ", Messages));
        }
    }
}
