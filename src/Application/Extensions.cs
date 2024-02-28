using IdGen;
using IdGen.DependencyInjection;
using JonathanPotts.RecipeCatalog.AI;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Services;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainServices();

        var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var generatorId = configuration.GetValue("GeneratorId", 0);
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

        var openAIConfiguration = configuration.GetSection("OpenAI");

        if (!string.IsNullOrEmpty(openAIConfiguration.GetValue<string>("ApiKey")))
        {
            services.AddAITextGenerator(openAIConfiguration);
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
