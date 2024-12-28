using auth.jwt.Services;
using Microsoft.Extensions.DependencyInjection;

namespace auth.jwt;

/// <summary>
/// Métodos de extensión para configuración de jwt
/// </summary>
public static class JwtServiceConfiguration
{
    /// <summary>
    /// Configuración de servicio jwt
    /// </summary>
    /// <param name="services"></param>
    /// <returns>Retorna service collection para que funcione como método de extensión</returns>
    public static IServiceCollection AddJWTServiceConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        return services;
    }
}