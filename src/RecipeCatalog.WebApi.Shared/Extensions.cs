using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeCatalog.Application;
using RecipeCatalog.Domain.Entities;
using RecipeCatalog.WebApi.Shared.Apis;
using RecipeCatalog.WebApi.Shared.Data;

namespace RecipeCatalog.WebApi.Shared;

public static class Extensions
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationServices(configuration);

        services.AddProblemDetails();

        return services;
    }

    public static IServiceCollection AddDbMigrator(this IServiceCollection services)
    {
        services.AddScoped<DbMigrator>();

        return services;
    }

    public static WebApplication UseDbMigrator(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<DbMigrator>().Migrate();
        }

        return app;
    }

    public static WebApplication MapWebApi(this WebApplication app)
    {
        app.MapGroup("/api/v1/identity").WithTags("Identity").MapIdentityApi<User>();

        app.MapCuisinesApi();
        app.MapRecipesApi();

        return app;
    }
}
