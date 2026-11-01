namespace Framework.DDD.Domain.Contract.Commons.ConfigurationModels;

public class SwaggerDocOptions
{
    public required string Name { get; set; }

    public required string Title { get; set; }

    public required string Version { get; set; }

    public string? Description { get; set; }
}