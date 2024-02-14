using JonathanPotts.RecipeCatalog.Application;
using JonathanPotts.RecipeCatalog.WebApi.Authorization;
using JonathanPotts.RecipeCatalog.WebApi.Data;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace JonathanPotts.RecipeCatalog.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, int generatorId = 0)
    {
        services.AddApplicationServices(generatorId);

        services.AddAuthorization();

        services.AddScoped<IAuthorizationHandler, CuisineAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RecipeAuthorizationHandler>();

        services.AddProblemDetails();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();

            c.AddSecurityDefinition("Identity", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            c.OperationFilter<SecurityRequirementsOperationFilter>(true, "Identity");
        });

        services.AddFluentValidationRulesToSwagger();

        return services;
    }

    public static IServiceCollection AddDbMigrator(this IServiceCollection services)
    {
        services.AddScoped<DbMigrator>();

        return services;
    }
}
