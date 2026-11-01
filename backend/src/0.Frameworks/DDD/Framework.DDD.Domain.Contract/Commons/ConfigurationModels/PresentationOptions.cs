namespace Framework.DDD.Domain.Contract.Commons.ConfigurationModels;

/// <summary>
/// Options for configuring the presentation layer.
/// </summary>
public class PresentationOptions
{
    public static PresentationOptions Instance { get; set; } = new();

    /// <summary>
    /// Whether to show exception detail in API responses. Should be false in production.
    /// </summary>
    public bool ShowExceptionDetail { get; set; }

    /// <summary>
    /// Whether to log API exceptions. Defaults to true.
    /// </summary>
    public bool ActiveApiLog { get; set; } = true;

    /// <summary>
    /// Allowed origins for CORS policy. If empty, all origins are allowed.
    /// </summary>
    public ICollection<PresentationCorsOptions> CorsPolicies { get; set; } = [];

    public ICollection<SwaggerDocOptions> SwaggerDocs { get; set; } = [];

}
