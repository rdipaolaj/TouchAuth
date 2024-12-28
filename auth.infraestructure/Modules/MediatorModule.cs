using auth.application;
using auth.handler.Login;
using auth.infraestructure.Behaviors;
using auth.internalservices;
using auth.jwt;
using auth.redis;
using auth.requestvalidator.Login;
using auth.secretmanager;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace auth.infraestructure.Modules;
public static class MediatorModule
{
    public static IServiceCollection AddMediatRAssemblyConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining(typeof(LoginCommandHandler));
            configuration.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(LoginCommandValidator).Assembly);

        return services;
    }
    public static IServiceCollection AddCustomServicesConfiguration(this IServiceCollection services)
    {
        services.AddInternalServicesConfiguration();
        services.AddSecretManagerConfiguration();
        services.AddRedisServiceConfiguration();
        services.AddApplicationConfiguration();
        services.AddJWTServiceConfiguration();

        return services;
    }
}