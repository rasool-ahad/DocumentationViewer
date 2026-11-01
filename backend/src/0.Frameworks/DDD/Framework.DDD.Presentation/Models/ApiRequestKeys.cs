namespace Framework.DDD.Presentation.Models;

/// <summary>
/// Well-known header/query keys used by the request/response middleware.
/// </summary>
public static class ApiRequestKeys
{
    public static class Headers
    {
        public const string ApiVersion = "X-Api-Version";
        public const string TraceId = "X-Trace-Id";
        public const string ClientContext = "X-Api-Client-Context";
        public const string ClientIp = "X-Forwarded-For";
        public const string Browser = "X-Browser";
        public const string UrlReferrer = "X-Url-Referrer";
    }

    public static class Queries
    {
        public const string ClientContext = "Context";
    }

    public static class Items
    {
        public const string ApiMessages = "ApiMessages";
    }
}
