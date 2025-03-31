using ErrorHandling.Service;

namespace ErrorHandling.Extensions;

public static class ErrorHandlingExtension
{
    public static IServiceCollection AddErrorHandlingService(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<ProblemDetailsService>();

        return services;
    }
}