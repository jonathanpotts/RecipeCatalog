using FluentValidation;
using IdGen;
using IdGen.DependencyInjection;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Services;
using JonathanPotts.RecipeCatalog.Application.Validation;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, int generatorId = 0)
    {
        services.AddDomainServices();

        services.AddValidatorsFromAssemblyContaining<RecipeDtoValidator>();

        var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        services.AddIdGen(generatorId, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

        services.AddScoped<IRecipesService, RecipesService>();

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
