using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeBook.AIDataGenerator;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorker(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<WorkerOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHostedService<Worker>();

        return services;
    }

    public static IServiceCollection AddWorker(this IServiceCollection services,
        Action<WorkerOptions> configureOptions)
    {
        services.AddOptions<WorkerOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHostedService<Worker>();

        return services;
    }
}
