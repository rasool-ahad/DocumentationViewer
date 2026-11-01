namespace Framework.DDD.Presentation.ApiResponses;

/// <summary>
/// Unified API response envelope.
/// </summary>
public class ApiResponse
{
    public string? RequestId { get; set; }
    public string? ApiVersion { get; set; }
    public string? Context { get; set; }
    public IEnumerable<ApiMessage>? Messages { get; set; }
    public ApiError? Error { get; set; }
    public ApiExceptionDetail? Exception { get; set; }

    public ApiResponse(string? apiVersion, string? requestId, string? context = null,
        IEnumerable<ApiMessage>? messages = null, ApiError? error = null, ApiExceptionDetail? exception = null)
    {
        ApiVersion = apiVersion;
        RequestId = requestId;
        Context = context;
        Messages = messages;
        Error = error;
        Exception = exception;
    }
}

/// <summary>
/// Unified API response envelope with a typed data payload.
/// </summary>
public class ApiResponse<TResult> : ApiResponse
{
    public TResult? Data { get; set; }

    public ApiResponse(string? apiVersion, string? requestId, string? context = null,
        IEnumerable<ApiMessage>? messages = null, TResult? data = default,
        ApiError? error = null, ApiExceptionDetail? exception = null)
        : base(apiVersion, requestId, context, messages, error, exception)
    {
        Data = data;
    }
}
