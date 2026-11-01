using Asp.Versioning;
using Framework.DDD.Domain.Contract.Commons.ConfigurationModels;
using Framework.DDD.Presentation.Injections.SwaggerOperationFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Framework.DDD.Presentation.Injections;

public static class Bootstrapper
{
    /// <summary>
    /// Registers presentation services and configures PresentationOptions.
    /// </summary>
    public static WebApplicationBuilder AddPresentationFramework(
        this WebApplicationBuilder builder,
        Action<PresentationOptions>? configure = null)
    {
        var options = new PresentationOptions();
        configure?.Invoke(options);
        PresentationOptions.Instance = options;

        builder.Host.ConfigureHost();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.ConfigureCors(options.CorsPolicies);
        builder.Services.ConfigureApiVersioning();
        builder.Services.ConfigureSwagger(options.SwaggerDocs);

        return builder;
    }

    private static void ConfigureHost(this IHostBuilder host)
    {
        host.UseDefaultServiceProvider(spo =>
        {
            spo.ValidateScopes = true;
            spo.ValidateOnBuild = true;
        });
    }

    private static void ConfigureCors(this IServiceCollection services, ICollection<PresentationCorsOptions> corsOptions)
    {
        services.AddCors(cors =>
        {
            foreach (var corsOption in corsOptions)
            {
                cors.AddPolicy(corsOption.PolicyName, policy =>
                {
                    if (corsOption.AllowOrigins.Length > 0)
                        policy.WithOrigins(corsOption.AllowOrigins);
                    else
                        policy.AllowAnyOrigin();

                    policy.AllowAnyMethod().AllowAnyHeader();
                });
            }
        });
    }

    private static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
            options.ReportApiVersions = false;
        }).AddMvc().AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services, ICollection<SwaggerDocOptions> swaggerDocOptions)
    {
        services.AddSwaggerGen(options =>
        {
            foreach (var item in swaggerDocOptions)
            {
                options.SwaggerDoc(item.Name, new OpenApiInfo { 
                    Title = item.Title, 
                    Version = item.Version, 
                    Description = item.Description });
            }
            options.EnableAnnotations();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(document => new() { [
                new OpenApiSecuritySchemeReference("Bearer", document)] = [] 
            });
            options.OperationFilter<ClientContextBodyOperationFilter>();
            options.DocumentFilter<RemoveSystemSchemasFilter>();
        });
    }
}
