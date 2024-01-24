using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeBook.DataGenerator.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTextGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<TextGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<TextGenerator>();

        return services;
    }

    public static IServiceCollection AddTextGenerator(this IServiceCollection services,
        Action<TextGeneratorOptions> configureOptions)
    {
        services.AddOptions<TextGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<TextGenerator>();

        return services;
    }

    public static IServiceCollection AddImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<ImageGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ImageGenerator>();

        return services;
    }

    public static IServiceCollection AddImageGenerator(this IServiceCollection services,
        Action<ImageGeneratorOptions> configureOptions)
    {
        services.AddOptions<ImageGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ImageGenerator>();

        return services;
    }
}
