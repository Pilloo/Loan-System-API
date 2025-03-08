using Core.UseCases.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class CoreServicesExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // services.AddTransient<IUserStore<User>, UserStore<User>>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            [
                typeof(LoginCommand).Assembly, typeof(RegisterCommand).Assembly,
                typeof(ConfirmEmailCommand).Assembly
            ]
        ));

        return services;
    }
}