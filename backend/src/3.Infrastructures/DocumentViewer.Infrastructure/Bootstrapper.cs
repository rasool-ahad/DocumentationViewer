using DocumentViewer.Infrastructure.Redis;
using DocumentViewer.Infrastructure.Sql;
using DocumentViewer.Contract.Options;
using DocumentViewer.Contract.Users.Admin.Services;
using DocumentViewer.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentViewer.Infrastructure;

public static class Bootstrapper
{
    public static IServiceCollection AddDocumentViewerInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDocumentViewerSqlInfrastructure(configuration);
        services.AddDocumentViewerRedisInfrastructure(configuration);
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer), "Jwt:Issuer is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Audience), "Jwt:Audience is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SigningKey) &&
                System.Text.Encoding.UTF8.GetByteCount(options.SigningKey) >= 32,
                "Jwt:SigningKey must be at least 32 UTF-8 bytes.")
            .Validate(options => options.AccessTokenLifetimeMinutes is > 0 and <= 1440,
                "Jwt:AccessTokenLifetimeMinutes must be between 1 and 1440.")
            .ValidateOnStart();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        return services;
    }
}

