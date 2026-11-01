using System.Net;

namespace Framework.DDD.Application.Exceptions;

/// <summary>
/// Represents a forbidden (403) in the application service layer.
/// </summary>
public class ApplicationForbiddenException : ApplicationServiceException
{
    public ApplicationForbiddenException(string message, Exception? innerException = null)
        : base(message, HttpStatusCode.Forbidden, innerException)
    {
    }
}
