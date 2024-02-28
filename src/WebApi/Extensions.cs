using JonathanPotts.RecipeCatalog.Application;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.WebApi.Apis;
using JonathanPotts.RecipeCatalog.WebApi.Data;

namespace JonathanPotts.RecipeCatalog.WebApi;

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
