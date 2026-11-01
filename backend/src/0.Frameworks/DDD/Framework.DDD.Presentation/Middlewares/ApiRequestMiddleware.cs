using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Framework.DDD.Presentation.Models;

namespace Framework.DDD.Presentation.Middlewares;

/// <summary>
/// Middleware that processes incoming API requests:
/// - Extracts client context from body wrapper (non-GET) or query string (GET)
/// - Sets client IP, browser, and referrer headers
/// </summary>
public class ApiRequestMiddleware(RequestDelegate next, ILogger<ApiRequestMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            SetIpToHeader(context);
            SetBrowserToHeader(context);
            SetUrlReferrerToHeader(context);
            await ExtractClientContextAsync(context);
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in ApiRequestMiddleware");
            throw;
        }
    }

    private static void SetIpToHeader(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrWhiteSpace(ip))
            context.Request.Headers[ApiRequestKeys.Headers.ClientIp] = ip;
    }

    private static void SetBrowserToHeader(HttpContext context)
    {
        var browser = context.Request.Headers.UserAgent.ToString();
        if (!string.IsNullOrWhiteSpace(browser))
            context.Request.Headers[ApiRequestKeys.Headers.Browser] = browser;
    }

    private static void SetUrlReferrerToHeader(HttpContext context)
    {
        var referer = context.Request.Headers.Referer.ToString();
        if (!string.IsNullOrWhiteSpace(referer))
            context.Request.Headers[ApiRequestKeys.Headers.UrlReferrer] = referer;
    }

    private static async Task ExtractClientContextAsync(HttpContext context)
    {
        if (context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            if (context.Request.Query.TryGetValue(ApiRequestKeys.Queries.ClientContext, out var clientContext)
                && !string.IsNullOrWhiteSpace(clientContext))
            {
                context.Request.Headers[ApiRequestKeys.Headers.ClientContext] = clientContext.ToString();
            }
        }
        else if (context.Request.HasJsonContentType())
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("context", out var contextProp))
                    {
                        var ctx = contextProp.GetString();
                        if (!string.IsNullOrWhiteSpace(ctx))
                            context.Request.Headers[ApiRequestKeys.Headers.ClientContext] = ctx;
                    }

                    if (root.TryGetProperty("data", out var dataProp))
                    {
                        var dataBytes = Encoding.UTF8.GetBytes(dataProp.GetRawText());
                        context.Request.Body = new MemoryStream(dataBytes);
                        context.Request.ContentLength = dataBytes.Length;
                    }
                    else
                    {
                        context.Request.Body.Position = 0;
                    }
                }
                catch (JsonException)
                {
                    context.Request.Body.Position = 0;
                }
            }
        }
    }
}

public static class ApiRequestMiddlewareExtension
{
    public static IApplicationBuilder UseApiRequestMiddleware(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ApiRequestMiddleware>();
}
