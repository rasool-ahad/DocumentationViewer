using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Framework.DDD.Application.Commands;
using Framework.DDD.Application.Events;
using Framework.DDD.Application.Queries;
using Framework.DDD.Domain.Contract.Services;

namespace Framework.DDD.Application.Injections;

public static class Bootstrapper
{
    /// <summary>
    /// Registers all application services: MediatR, CQRS buses, FluentValidation, and Scrutor-based DI scanning.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for handlers, validators, and services.</param>
    public static IServiceCollection AddApplicationFramework(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        // MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
        });

        // CQRS buses
        services.AddTransient<ICommandBus, CommandBus>();
        services.AddTransient<IQueryBus, QueryBus>();
        services.AddTransient<IEventBus, EventBus>();
        services.AddTransient<ICommandQueryBus, CommandQueryBus>();

        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblies(assemblies);

        // Scrutor — marker-based lifetime registration
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo<ITransientService>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo<IScopedService>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo<ISingletonService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        return services;
    }
}
