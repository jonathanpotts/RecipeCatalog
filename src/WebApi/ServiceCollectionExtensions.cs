using FluentValidation;
using IdGen;
using IdGen.DependencyInjection;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.WebApi.Authorization;
using JonathanPotts.RecipeCatalog.WebApi.Data;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace JonathanPotts.RecipeCatalog.WebApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services, int generatorId = 0)
    {
        // Add services to the container.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(
                $"Data Source={Path.Combine(AppContext.BaseDirectory, $"{nameof(ApplicationDbContext)}.db")}"));

        services.AddAuthorization();

        services.AddScoped<IAuthorizationHandler, CuisineAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, RecipeAuthorizationHandler>();

        services.AddValidatorsFromAssemblyContaining<Program>();

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

        // Add IdGen for creating Snowflake IDs
        var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        services.AddIdGen(generatorId, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

        return services;
    }

    public static IServiceCollection AddDbMigrator(this IServiceCollection services)
    {
        services.AddScoped<DbMigrator>();

        return services;
    }
}
