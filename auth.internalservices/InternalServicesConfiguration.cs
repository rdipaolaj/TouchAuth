using auth.internalservices.Base;
using auth.internalservices.Rol;
using auth.internalservices.User;
using Microsoft.Extensions.DependencyInjection;

namespace auth.internalservices;
public static class InternalServicesConfiguration
{
    public static IServiceCollection AddInternalServicesConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IBaseService, BaseService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IUserService, UserService>();

        return services;
    }
}