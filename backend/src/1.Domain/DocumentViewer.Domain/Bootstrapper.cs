using Microsoft.Extensions.DependencyInjection;

namespace DocumentViewer.Domain;

public static class Bootstrapper
{
    public static IServiceCollection AddDocumentViewerDomain(this IServiceCollection services)
    {
        // Entities are created by the domain or materialized by EF Core; they are not DI services.
        return services;
    }
}

