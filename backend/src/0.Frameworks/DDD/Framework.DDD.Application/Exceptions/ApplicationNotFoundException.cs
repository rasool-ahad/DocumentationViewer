using System.Net;

namespace Framework.DDD.Application.Exceptions;

/// <summary>
/// Represents a not found (404) in the application service layer.
/// </summary>
public class ApplicationNotFoundException : ApplicationServiceException
{
    public ApplicationNotFoundException(string message, Exception? innerException = null)
        : base(message, HttpStatusCode.NotFound, innerException)
    {
    }
}
