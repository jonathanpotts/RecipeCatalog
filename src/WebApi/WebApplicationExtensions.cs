using JonathanPotts.RecipeCatalog.WebApi.Apis;
using JonathanPotts.RecipeCatalog.WebApi.Data;
using Microsoft.AspNetCore.Identity;

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
        app.MapGroup("/api/v1/identity").WithTags("Identity").MapIdentityApi<IdentityUser>();

        app.MapCuisinesApi();
        app.MapRecipesApi();

        return app;
    }
}
