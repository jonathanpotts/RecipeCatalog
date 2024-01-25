using JonathanPotts.RecipeCatalog.WebApi.Data.EntityConfigurations;
using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.WebApi.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Cuisine> Cuisines => Set<Cuisine>();

    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CuisineEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeEntityTypeConfiguration());
    }
}
