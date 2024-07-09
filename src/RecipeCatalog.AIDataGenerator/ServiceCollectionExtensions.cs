using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeCatalog.AIDataGenerator;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorker(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<Options.Worker>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHostedService<Worker>();

        return services;
    }

    public static IServiceCollection AddWorker(this IServiceCollection services,
        Action<Options.Worker> configureOptions)
    {
        services.AddOptions<Options.Worker>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHostedService<Worker>();

        return services;
    }
}
