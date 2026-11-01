using DocumentViewer.Infrastructure.Redis;
using DocumentViewer.Infrastructure.Sql;
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
        return services;
    }
}

