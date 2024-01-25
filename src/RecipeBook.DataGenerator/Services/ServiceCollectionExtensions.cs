using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeBook.DataGenerator.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAITextGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<OpenAITextGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITextGenerator, OpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddOpenAITextGenerator(this IServiceCollection services,
        Action<OpenAITextGeneratorOptions> configureOptions)
    {
        services.AddOptions<OpenAITextGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITextGenerator, OpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAITextGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<AzureOpenAITextGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITextGenerator, AzureOpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAITextGenerator(this IServiceCollection services,
        Action<AzureOpenAITextGeneratorOptions> configureOptions)
    {
        services.AddOptions<AzureOpenAITextGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITextGenerator, AzureOpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddOpenAIImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<OpenAIImageGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageGenerator, OpenAIImageGenerator>();

        return services;
    }

    public static IServiceCollection AddOpenAIImageGenerator(this IServiceCollection services,
        Action<OpenAIImageGeneratorOptions> configureOptions)
    {
        services.AddOptions<OpenAIImageGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageGenerator, OpenAIImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIDallE2ImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<AzureOpenAIDallE2ImageGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageGenerator, AzureOpenAIDallE2ImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIDallE2ImageGenerator(this IServiceCollection services,
        Action<AzureOpenAIDallE2ImageGeneratorOptions> configureOptions)
    {
        services.AddOptions<AzureOpenAIDallE2ImageGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageGenerator, AzureOpenAIDallE2ImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<AzureOpenAIImageGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageGenerator, AzureOpenAIImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIImageGenerator(this IServiceCollection services,
        Action<AzureOpenAIImageGeneratorOptions> configureOptions)
    {
        services.AddOptions<AzureOpenAIImageGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IImageGenerator, AzureOpenAIImageGenerator>();

        return services;
    }
}
