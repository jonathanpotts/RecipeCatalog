using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeCatalog.Domain.Entities;
using RecipeCatalog.Domain.EntityConfigurations;

namespace RecipeCatalog.Domain;

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
