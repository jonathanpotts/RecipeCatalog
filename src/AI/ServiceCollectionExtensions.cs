﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.AI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAITextGenerator(this IServiceCollection services,
        IConfiguration namedConfigurationSection)
    {
        if (!string.IsNullOrWhiteSpace(namedConfigurationSection.GetValue<string>("Endpoint")))
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
        if (!string.IsNullOrWhiteSpace(namedConfigurationSection.GetValue<string>("Endpoint")))
        {
            services.AddAzureOpenAIImageGenerator(namedConfigurationSection);
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
