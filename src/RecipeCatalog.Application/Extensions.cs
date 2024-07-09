using IdGen;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using RecipeCatalog.Application.Authorization;
using RecipeCatalog.Application.Contracts.Authorization;
using RecipeCatalog.Application.Contracts.Services;
using RecipeCatalog.Application.Options;
using RecipeCatalog.Application.Services;
using RecipeCatalog.Domain;
using RecipeCatalog.Domain.Entities;

namespace RecipeCatalog.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainServices();

        var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var generatorId = configuration.GetValue<int?>("GeneratorId") ?? 0;
        services.AddIdGen(generatorId, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

        services.AddScoped<ICuisineService, CuisineService>();
        services.AddScoped<IRecipeService, RecipeService>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.Create, policy =>
                policy.AddRequirements(Operations.Create))
            .AddPolicy(Policies.Read, policy =>
                policy.AddRequirements(Operations.Read))
            .AddPolicy(Policies.Update, policy =>
                policy.AddRequirements(Operations.Update))
            .AddPolicy(Policies.Delete, policy =>
                policy.AddRequirements(Operations.Delete));

        services.AddScoped<IAuthorizationHandler, CuisineAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, CuisineDtoAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RecipeAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RecipeDtoAuthorizationHandler>();

        var textEmbeddingConfiguration = configuration.GetSection("OpenAI:TextEmbedding");

        if (!string.IsNullOrEmpty(textEmbeddingConfiguration.GetValue<string>("Key")))
        {
            services.AddOptions<TextEmbedding>()
                .Bind(textEmbeddingConfiguration)
                .ValidateDataAnnotations()
                .ValidateOnStart();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            services.AddSingleton<ITextEmbeddingGenerationService>(services =>
            {
                var options = services.GetRequiredService<IOptions<TextEmbedding>>().Value;

                if (string.IsNullOrEmpty(options.Endpoint))
                {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                    return new OpenAITextEmbeddingGenerationService(options.Model, options.Key);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                }
                else
                {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                    return new AzureOpenAITextEmbeddingGenerationService(options.Model, options.Endpoint, options.Key);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                }
            });
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        return services;
    }

    public static IServiceCollection AddIdentityApiEndpoints(this IServiceCollection services)
    {
        services.AddIdentityApiEndpoints<User>()
            .AddRoles<IdentityRole>()
            .AddDomainStores();

        return services;
    }

    public static IServiceCollection AddIdentityBlazor(this IServiceCollection services)
    {
        services.AddIdentityApiEndpoints();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        });

        return services;
    }
}
