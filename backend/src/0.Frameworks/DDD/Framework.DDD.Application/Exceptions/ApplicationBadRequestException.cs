using System.Net;

namespace Framework.DDD.Application.Exceptions;

/// <summary>
/// Represents a bad request (400) in the application service layer.
/// </summary>
public class ApplicationBadRequestException : ApplicationServiceException
{
    public ApplicationBadRequestException(string message, Exception? innerException = null)
        : base(message, HttpStatusCode.BadRequest, innerException)
    {
    }
}
