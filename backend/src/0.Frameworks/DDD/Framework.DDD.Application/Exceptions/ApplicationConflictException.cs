using System.Net;

namespace Framework.DDD.Application.Exceptions;

/// <summary>
/// Represents a conflict (409) in the application service layer.
/// </summary>
public class ApplicationConflictException : ApplicationServiceException
{
    public ApplicationConflictException(string message, Exception? innerException = null)
        : base(message, HttpStatusCode.Conflict, innerException)
    {
    }
}
