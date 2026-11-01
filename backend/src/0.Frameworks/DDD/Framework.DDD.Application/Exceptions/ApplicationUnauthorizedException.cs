using System.Net;

namespace Framework.DDD.Application.Exceptions;

/// <summary>
/// Represents an unauthorized (401) in the application service layer.
/// </summary>
public class ApplicationUnauthorizedException : ApplicationServiceException
{
    public ApplicationUnauthorizedException(string message, Exception? innerException = null)
        : base(message, HttpStatusCode.Unauthorized, innerException)
    {
    }
}
