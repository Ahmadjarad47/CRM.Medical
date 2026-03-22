using System.Reflection;
using CRM.Medical.Application.Behaviors;
using CRM.Medical.Application.Common.Time;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Medical.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] additionalMappingAssemblies)
    {
        var mappingAssemblies = new[] { Assembly.GetExecutingAssembly() }
            .Concat(additionalMappingAssemblies)
            .Distinct()
            .ToArray();

        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(mappingAssemblies);
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, ServiceMapper>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Outer runs first: logging → activity tags → validation → handler.
            cfg.AddOpenBehavior(typeof(LoggingPipelineBehavior<,>), ServiceLifetime.Transient);
            cfg.AddOpenBehavior(typeof(ActivityTaggingPipelineBehavior<,>), ServiceLifetime.Transient);
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>), ServiceLifetime.Transient);
        });

        return services;
    }
}
