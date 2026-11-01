namespace Framework.ResultPattern;

public sealed record ResultError(string Code, string Message)
{
    public static readonly ResultError None = new(string.Empty, string.Empty);

    public static ResultError Create(string code, string message) => new(code, message);
}
