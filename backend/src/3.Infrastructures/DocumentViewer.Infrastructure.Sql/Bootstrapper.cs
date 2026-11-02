using DocumentViewer.Contract.Options;
using DocumentViewer.Domain.Users;
using DocumentViewer.Infrastructure.Sql.Commons.Context;
using Framework.DDD.Domain.Toolkits.Entities.Identity;
using Framework.DDD.Infrastructure.Sql.Interceptors;
using Framework.DDD.Infrastructure.Sql.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DocumentViewer.Infrastructure.Sql;

public static class Bootstrapper
{
    public static IServiceCollection AddDocumentViewerSqlInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SqlDatabaseOptions>()
            .Bind(configuration.GetSection(SqlDatabaseOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString),
                "SqlDatabase:ConnectionString is required.")
            .ValidateOnStart();

        services.AddOptions<IdentityOptions>()
            .Bind(configuration.GetSection("Identity"));

        services.AddOptions<SuperAdminOptions>()
            .Bind(configuration.GetSection(SuperAdminOptions.SectionName));

        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddDbContext<DocumentViewerIdentityDbContext>((provider, options) =>
        {
            var database = provider.GetRequiredService<IOptions<SqlDatabaseOptions>>().Value;
            options.UseSqlServer(database.ConnectionString);
            options.AddInterceptors(provider.GetRequiredService<PublishDomainEventsInterceptor>());
        });

        services.AddScoped<IUnitOfWork<int>>(
            provider => provider.GetRequiredService<DocumentViewerIdentityDbContext>());

        services.AddIdentityCore<User>()
            .AddRoles<BaseRole<int>>()
            .AddSignInManager()
            .AddEntityFrameworkStores<DocumentViewerIdentityDbContext>();

        return services;
    }
}

