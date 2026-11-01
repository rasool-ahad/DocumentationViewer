using System.Text.RegularExpressions;
using Framework.DDD.Domain.ValueObjects;

namespace Framework.DDD.Domain.Toolkits.ValueObjects.Rules;

public partial class IranianPhoneNumberMustBeValidRule : IBusinessRule
{
    private readonly string _value;
    public List<string> Messages { get; set; } = [];

    public IranianPhoneNumberMustBeValidRule(string value) => _value = value;

    public void IsBroken()
    {
        if (string.IsNullOrWhiteSpace(_value))
        {
            Messages.Add("Iranian phone number is required.");
            throw new ArgumentException(string.Join(", ", Messages));
        }
        if (!PhoneRegex().IsMatch(_value))
        {
            Messages.Add("Iranian phone number format is invalid. Expected: 09xxxxxxxxx or +989xxxxxxxxx.");
            throw new ArgumentException(string.Join(", ", Messages));
        }
    }

    [GeneratedRegex(@"^(\+98|0)?9\d{9}$")]
    private static partial Regex PhoneRegex();
}
