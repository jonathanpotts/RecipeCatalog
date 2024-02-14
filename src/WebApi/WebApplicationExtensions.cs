using JonathanPotts.RecipeCatalog.Domain.Models;
using JonathanPotts.RecipeCatalog.WebApi.Apis;
using JonathanPotts.RecipeCatalog.WebApi.Data;

namespace JonathanPotts.RecipeCatalog.WebApi;

public static class WebApplicationExtensions
{
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
        app.MapGroup("/api/v1/identity").WithTags("Identity").MapIdentityApi<ApplicationUser>();

        app.MapCuisinesApi();
        app.MapRecipesApi();

        return app;
    }
}
