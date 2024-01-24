using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeBook.DataGenerator.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTextGenerationService(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<TextGenerationServiceOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<TextGenerationService>();

        return services;
    }

    public static IServiceCollection AddTextGenerationService(this IServiceCollection services,
        Action<TextGenerationServiceOptions> configureOptions)
    {
        services.AddOptions<TextGenerationServiceOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<TextGenerationService>();

        return services;
    }

    public static IServiceCollection AddImageGenerationService(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<ImageGenerationServiceOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ImageGenerationService>();

        return services;
    }

    public static IServiceCollection AddImageGenerationService(this IServiceCollection services,
        Action<ImageGenerationServiceOptions> configureOptions)
    {
        services.AddOptions<ImageGenerationServiceOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ImageGenerationService>();

        return services;
    }
}
