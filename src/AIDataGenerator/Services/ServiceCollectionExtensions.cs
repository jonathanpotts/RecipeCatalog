using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAITextGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        if (namedConfigurationSection.GetValue<bool>("UseAzureOpenAI"))
        {
            services.AddAzureOpenAITextGenerator(namedConfigurationSection);
        }
        else
        {
            services.AddOpenAITextGenerator(namedConfigurationSection);
        }

        return services;
    }

    public static IServiceCollection AddAIImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        if (namedConfigurationSection.GetValue<bool>("UseAzureOpenAI"))
        {
            if (namedConfigurationSection.GetValue<bool>("UseDallE3"))
            {
                services.AddAzureOpenAIImageGenerator(namedConfigurationSection);
            }
            else
            {
                services.AddAzureOpenAIDallE2ImageGenerator(namedConfigurationSection);
            }
        }
        else
        {
            services.AddOpenAIImageGenerator(namedConfigurationSection);
        }

        return services;
    }

    public static IServiceCollection AddOpenAITextGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<OpenAITextGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAITextGenerator, OpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddOpenAITextGenerator(this IServiceCollection services,
        Action<OpenAITextGeneratorOptions> configureOptions)
    {
        services.AddOptions<OpenAITextGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAITextGenerator, OpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAITextGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<AzureOpenAITextGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAITextGenerator, AzureOpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAITextGenerator(this IServiceCollection services,
        Action<AzureOpenAITextGeneratorOptions> configureOptions)
    {
        services.AddOptions<AzureOpenAITextGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAITextGenerator, AzureOpenAITextGenerator>();

        return services;
    }

    public static IServiceCollection AddOpenAIImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<OpenAIImageGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAIImageGenerator, OpenAIImageGenerator>();

        return services;
    }

    public static IServiceCollection AddOpenAIImageGenerator(this IServiceCollection services,
        Action<OpenAIImageGeneratorOptions> configureOptions)
    {
        services.AddOptions<OpenAIImageGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAIImageGenerator, OpenAIImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIDallE2ImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<AzureOpenAIDallE2ImageGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAIImageGenerator, AzureOpenAIDallE2ImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIDallE2ImageGenerator(this IServiceCollection services,
        Action<AzureOpenAIDallE2ImageGeneratorOptions> configureOptions)
    {
        services.AddOptions<AzureOpenAIDallE2ImageGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAIImageGenerator, AzureOpenAIDallE2ImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIImageGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        services.AddOptions<AzureOpenAIImageGeneratorOptions>()
            .Bind(namedConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAIImageGenerator, AzureOpenAIImageGenerator>();

        return services;
    }

    public static IServiceCollection AddAzureOpenAIImageGenerator(this IServiceCollection services,
        Action<AzureOpenAIImageGeneratorOptions> configureOptions)
    {
        services.AddOptions<AzureOpenAIImageGeneratorOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IAIImageGenerator, AzureOpenAIImageGenerator>();

        return services;
    }
}
