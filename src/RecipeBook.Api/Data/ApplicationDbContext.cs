using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data.EntityConfigurations;
using RecipeBook.Api.Models;

namespace RecipeBook.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RecipeEntityTypeConfiguration());
    }
}