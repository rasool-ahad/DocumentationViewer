using System.Net;

namespace Framework.DDD.Application.Exceptions;

/// <summary>
/// Base exception for the application service layer.
/// </summary>
public class ApplicationServiceException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApplicationServiceException(string message, HttpStatusCode statusCode, Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
