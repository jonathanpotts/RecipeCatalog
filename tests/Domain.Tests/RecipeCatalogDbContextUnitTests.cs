using JonathanPotts.RecipeCatalog.Tests.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Domain.Tests;

public sealed class RecipeCatalogDbContextUnitTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<RecipeCatalogDbContext> _contextOptions;

    private RecipeCatalogDbContext CreateContext() => new(_contextOptions);

    public RecipeCatalogDbContextUnitTests()
    {
        _connection = new("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<RecipeCatalogDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = CreateContext();
        context.Database.EnsureCreated();

        context.Users.AddRange(TestData.Users);
        context.Roles.AddRange(TestData.Roles);
        context.UserRoles.AddRange(TestData.UserRoles);
        context.Cuisines.AddRange(TestData.Cuisines);
        context.Recipes.AddRange(TestData.Recipes);
        context.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    [Fact]
    public void RecipeHasRelatedData()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var recipe = context.Recipes
            .Include(x => x.Owner)
            .Include(x => x.Cuisine)
            .First();

        // Assert
        Assert.NotNull(recipe);
        Assert.NotNull(recipe.Owner);
        Assert.NotNull(recipe.Cuisine);
    }

    [Fact]
    public void CuisineHasRelatedData()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var cuisine = context.Cuisines
            .Include(x => x.Recipes)
            .First(x => x.Id == TestData.Recipes[0].CuisineId);

        // Assert
        Assert.NotNull(cuisine);
        Assert.NotNull(cuisine.Recipes);
    }

    [Fact]
    public void UserHasRelatedData()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var user = context.Users
            .Include(x => x.Recipes)
            .First(x => x.Id == TestData.Recipes[0].OwnerId);

        // Assert
        Assert.NotNull(user);
        Assert.NotNull(user.Recipes);
    }
}
