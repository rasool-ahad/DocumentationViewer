namespace Framework.DDD.Presentation.ApiResponses;

/// <summary>
/// Exception detail included in API responses when ShowExceptionDetail is enabled.
/// </summary>
public class ApiExceptionDetail
{
    public string? Message { get; set; }
    public string? StackTrace { get; set; }

    public ApiExceptionDetail()
    {
    }

    public ApiExceptionDetail(string? message, string? stackTrace = null)
    {
        Message = message;
        StackTrace = stackTrace;
    }
}
