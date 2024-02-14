using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.Domain;

public static class Extensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddDbContext<RecipeCatalogDbContext>(options =>
            options.UseSqlite(
                $"Data Source={Path.Combine(AppContext.BaseDirectory, $"{nameof(RecipeCatalogDbContext)}.db")}"));

        services.AddScoped<IRepository<Cuisine>, CuisinesRepository>();
        services.AddScoped<IRepository<Recipe>, RecipesRepository>();

        return services;
    }

    public static IdentityBuilder AddDomainStores(this IdentityBuilder builder)
    {
        builder.AddEntityFrameworkStores<RecipeCatalogDbContext>();

        return builder;
    }
}
