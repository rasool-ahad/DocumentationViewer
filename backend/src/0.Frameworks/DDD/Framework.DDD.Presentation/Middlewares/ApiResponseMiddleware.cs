using Framework.DDD.Application.Exceptions;
using Framework.DDD.Domain.Contract.Commons.ConfigurationModels;
using Framework.DDD.Presentation.ApiResponses;
using Framework.DDD.Presentation.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Framework.DDD.Presentation.Middlewares;

/// <summary>
/// Middleware that wraps all API responses into a unified ApiResponse envelope.
/// Handles success, non-success status codes, and exceptions.
/// </summary>
public class ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    private readonly PresentationOptions _options = PresentationOptions.Instance;

    public async Task Invoke(HttpContext context)
    {
        if (!ShouldWrapResponse(context))
        {
            await next(context);
            return;
        }

        var originalBody = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        var responseHandled = false;

        try
        {
            await next(context);

            // Skip wrapping for file/download responses
            var contentDisposition = context.Response.Headers.ContentDisposition.ToString();
            if (!string.IsNullOrWhiteSpace(contentDisposition) && contentDisposition.Contains("attachment"))
            {
                context.Response.Body = originalBody;
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBody);
                responseHandled = true;
                return;
            }

            var version = context.Request.Headers[ApiRequestKeys.Headers.ApiVersion].ToString();
            var traceId = context.TraceIdentifier;
            var clientContext = context.Request.Headers[ApiRequestKeys.Headers.ClientContext].ToString();

            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                await HandleSuccessAsync(context, originalBody, version, traceId, clientContext);
                responseHandled = true;
            }
            else if (context.Response.StatusCode != 302)
            {
                await HandleNonSuccessAsync(context, originalBody, version, traceId, clientContext);
                responseHandled = true;
            }
        }
        catch (Exception ex)
        {
            if (_options.ActiveApiLog)
                LogException(ex, context.Request.Path);

            var version = context.Request.Headers[ApiRequestKeys.Headers.ApiVersion].ToString();
            var traceId = context.TraceIdentifier;
            var clientContext = context.Request.Headers[ApiRequestKeys.Headers.ClientContext].ToString();

            context.Response.Body = originalBody;
            await HandleExceptionAsync(context, ex, version, traceId, clientContext);
            responseHandled = true;
        }
        finally
        {
            if (!responseHandled && context.Response.StatusCode != StatusCodes.Status204NoContent)
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBody);
            }
        }
    }

    private async Task HandleSuccessAsync(HttpContext context, Stream originalBody, string version, string traceId, string clientContext)
    {
        var body = await ReadResponseBody(context.Response);
        context.Response.Body = originalBody;
        context.Response.ContentType = "application/json";

        object? data = null;
        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                data = JsonSerializer.Deserialize<JsonElement>(body);
            }
            catch
            {
                data = body;
            }
        }

        var messages = GetMessages(context);
        var apiResponse = new ApiResponse<object>(version, traceId, clientContext, messages, data);
        await WriteResponseAsync(context.Response, apiResponse, context.Response.StatusCode);
    }

    private async Task HandleNonSuccessAsync(HttpContext context, Stream originalBody, string version, string traceId, string clientContext)
    {
        var body = await ReadResponseBody(context.Response);
        var statusCode = context.Response.StatusCode;

        context.Response.Body = originalBody;
        context.Response.ContentType = "application/json";

        var apiError = new ApiError(((HttpStatusCode)statusCode).ToString(), statusCode, ParseValidationErrors(body));
        var messages = GetMessages(context);
        var apiResponse = new ApiResponse(version, traceId, clientContext, messages, apiError);

        await WriteResponseAsync(context.Response, apiResponse, statusCode);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, string version, string traceId, string clientContext)
    {
        context.Response.ContentType = "application/json";

        var messages = GetMessages(context);
        ApiError apiError;
        ApiExceptionDetail? exceptionDetail = null;
        int statusCode;

        if (exception is ApplicationServiceException appEx)
        {
            statusCode = (int)appEx.StatusCode;
            apiError = new ApiError(appEx.StatusCode.ToString(), statusCode,
                [new ValidationError(null, appEx.Message)]);
            messages ??= [];
        }
        else if (exception is UnauthorizedAccessException)
        {
            statusCode = StatusCodes.Status401Unauthorized;
            apiError = new ApiError("Unauthorized", statusCode);
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            apiError = new ApiError("InternalServerError", statusCode);
        }

        if (_options.ShowExceptionDetail)
        {
            exceptionDetail = new ApiExceptionDetail(exception.Message, exception.StackTrace);
        }

        var apiResponse = new ApiResponse(version, traceId, clientContext, messages, apiError, exceptionDetail);
        await WriteResponseAsync(context.Response, apiResponse, statusCode);
    }

    private static async Task WriteResponseAsync(HttpResponse response, object apiResponse, int statusCode)
    {
        response.StatusCode = statusCode;
        var json = JsonSerializer.Serialize(apiResponse, JsonOptions);
        await response.WriteAsync(json);
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return text;
    }

    private static List<ApiMessage>? GetMessages(HttpContext context)
    {
        if (context.Items.TryGetValue(ApiRequestKeys.Items.ApiMessages, out var obj) && obj is List<ApiMessage> messages)
            return messages;
        return null;
    }

    private static List<ValidationError>? ParseValidationErrors(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (!doc.RootElement.TryGetProperty("errors", out var errors))
                return null;

            var result = new List<ValidationError>();
            foreach (var prop in errors.EnumerateObject())
            {
                foreach (var msg in prop.Value.EnumerateArray())
                {
                    result.Add(new ValidationError(prop.Name, msg.GetString()));
                }
            }
            return result.Count > 0 ? result : null;
        }
        catch
        {
            return null;
        }
    }

    private static bool ShouldWrapResponse(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        if (path.Contains("/swagger", StringComparison.OrdinalIgnoreCase))
            return false;
        if (path.EndsWith("/health", StringComparison.OrdinalIgnoreCase))
            return false;
        if (path.EndsWith("/ping", StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    private void LogException(Exception ex, string path)
    {
        logger.LogError(ex, "Exception at {Path}: {Message}", path, ex.Message);
    }
}

public static class ApiResponseMiddlewareExtension
{
    public static IApplicationBuilder UseApiResponseMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ApiResponseMiddleware>();
}
