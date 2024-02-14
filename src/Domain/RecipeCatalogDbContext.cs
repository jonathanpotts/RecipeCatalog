using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Domain;

public class RecipeCatalogDbContext(DbContextOptions<RecipeCatalogDbContext> options)
    : IdentityDbContext<User>(options)
{
    public DbSet<Cuisine> Cuisines => Set<Cuisine>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CuisineEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeEntityTypeConfiguration());
    }
}
