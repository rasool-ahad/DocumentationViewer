namespace Framework.DDD.Presentation.ApiResponses;

/// <summary>
/// Represents an error in the API response.
/// </summary>
public class ApiError
{
    public string? Code { get; set; }
    public int? Status { get; set; }
    public IEnumerable<ValidationError>? Details { get; set; }

    public ApiError()
    {
    }

    public ApiError(string? code, int? status = null, IEnumerable<ValidationError>? details = null)
    {
        Code = code;
        Status = status;
        Details = details;
    }
}

/// <summary>
/// Represents a single validation error detail.
/// </summary>
public class ValidationError
{
    public string? Field { get; set; }
    public string? Message { get; set; }

    public ValidationError()
    {
    }

    public ValidationError(string? field, string? message)
    {
        Field = field;
        Message = message;
    }
}
