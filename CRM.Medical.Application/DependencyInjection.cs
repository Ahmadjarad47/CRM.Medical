using System.Reflection;
using CRM.Medical.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Medical.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
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
