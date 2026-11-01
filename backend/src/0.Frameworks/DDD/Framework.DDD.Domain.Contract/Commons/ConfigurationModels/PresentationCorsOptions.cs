namespace Framework.DDD.Domain.Contract.Commons.ConfigurationModels;

public class PresentationCorsOptions
{
    public string[] AllowOrigins { get; set; } = [];

    public required string PolicyName { get; set; }
}