using auth.secretmanager.Service;
using Microsoft.Extensions.DependencyInjection;

namespace auth.secretmanager;
public static class SecretManagerConfiguration
{
    public static IServiceCollection AddSecretManagerConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<ISecretManagerService, SecretManagerService>();

        return services;
    }
}