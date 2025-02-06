using Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions;

public static class CoreServicesExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // services.AddTransient<IUserStore<User>, UserStore<User>>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            [
                typeof(UseCases.Commands.LoginCommand).Assembly, typeof(UseCases.Commands.RegisterCommand).Assembly,
                typeof(UseCases.Commands.VerifyEmailCommand).Assembly
            ]
        ));

        return services;
    }
}