using JonathanPotts.RecipeBook.WebApi.Data.EntityConfigurations;
using JonathanPotts.RecipeBook.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeBook.WebApi.Data;

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
