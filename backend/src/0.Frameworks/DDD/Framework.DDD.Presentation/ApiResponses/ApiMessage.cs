namespace Framework.DDD.Presentation.ApiResponses;

/// <summary>
/// A message included in the API response (info, success, warning, error).
/// </summary>
public class ApiMessage
{
    public ApiMessageType Type { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public ApiMessage()
    {
    }

    public ApiMessage(ApiMessageType type, string? title, string? description = null)
    {
        Type = type;
        Title = title;
        Description = description;
    }
}

public enum ApiMessageType : byte
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3
}
