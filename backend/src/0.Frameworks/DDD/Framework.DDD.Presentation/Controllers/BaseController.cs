using Microsoft.AspNetCore.Mvc;
using Framework.DDD.Presentation.ApiResponses;
using Framework.DDD.Presentation.Models;

namespace Framework.DDD.Presentation.Controllers;

/// <summary>
/// Base API controller providing message helpers and request context properties.
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// The trace identifier for the current request.
    /// </summary>
    protected string TraceId => HttpContext.TraceIdentifier;

    /// <summary>
    /// The API version from the request header.
    /// </summary>
    protected string? ApiVersion => HttpContext.Request.Headers[ApiRequestKeys.Headers.ApiVersion].ToString();

    /// <summary>
    /// The client context from the request header.
    /// </summary>
    protected string? ClientContext => HttpContext.Request.Headers[ApiRequestKeys.Headers.ClientContext].ToString();

    /// <summary>
    /// The client IP address from the request header.
    /// </summary>
    protected string? ClientIp => HttpContext.Request.Headers[ApiRequestKeys.Headers.ClientIp].ToString();

    /// <summary>
    /// The client browser (User-Agent) from the request header.
    /// </summary>
    protected string? Browser => HttpContext.Request.Headers[ApiRequestKeys.Headers.Browser].ToString();

    /// <summary>
    /// Adds an info message to the response.
    /// </summary>
    protected void AddInfoMessage(string title, string? description = null) =>
        AddMessage(new ApiMessage(ApiMessageType.Info, title, description));

    /// <summary>
    /// Adds a success message to the response.
    /// </summary>
    protected void AddSuccessMessage(string title, string? description = null) =>
        AddMessage(new ApiMessage(ApiMessageType.Success, title, description));

    /// <summary>
    /// Adds a warning message to the response.
    /// </summary>
    protected void AddWarningMessage(string title, string? description = null) =>
        AddMessage(new ApiMessage(ApiMessageType.Warning, title, description));

    /// <summary>
    /// Adds an error message to the response.
    /// </summary>
    protected void AddErrorMessage(string title, string? description = null) =>
        AddMessage(new ApiMessage(ApiMessageType.Error, title, description));

    private void AddMessage(ApiMessage message)
    {
        if (!HttpContext.Items.TryGetValue(ApiRequestKeys.Items.ApiMessages, out var obj)
            || obj is not List<ApiMessage> messages)
        {
            messages = [];
            HttpContext.Items[ApiRequestKeys.Items.ApiMessages] = messages;
        }

        messages.Add(message);
    }
}
