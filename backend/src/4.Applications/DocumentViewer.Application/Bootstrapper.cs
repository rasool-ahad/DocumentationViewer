using DocumentViewer.Domain;
using Framework.DDD.Application.Injections;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentViewer.Application;

public static class Bootstrapper
{
    public static IServiceCollection AddDocumentViewerApplication(this IServiceCollection services)
    {
        services.AddDocumentViewerDomain();
        services.AddApplicationFramework(typeof(Bootstrapper).Assembly);
        return services;
    }
}

