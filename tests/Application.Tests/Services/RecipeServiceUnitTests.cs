using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Services;
using JonathanPotts.RecipeCatalog.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Services;

public sealed class RecipeServiceUnitTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<RecipeCatalogDbContext> _contextOptions;

    private RecipeCatalogDbContext CreateContext() => new(_contextOptions);

    private RecipeService CreateRecipeService() => new(
        CreateContext(),
        Mocks.CreateIdGeneratorMock().Object,
        Mocks.CreateUserManagerMock().Object,
        Mock.Of<IAuthorizationService>(),
        Mock.Of<IServiceProvider>());

    public RecipeServiceUnitTests()
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
    public async void GetListAsyncReturnsPagedResult()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(Math.Min(TestData.Recipes.Length, IRecipeService.MaxItemsPerPage), result.Items.Count());
        Assert.Equal(TestData.Recipes.Length, result.Total);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async void GetListAsyncReturnsPagedResultWithSkip(int skip)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetListAsync(skip);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(
            TestData.Recipes.OrderByDescending(x => x.Id).Skip(skip).First().Id,
            result.Items.First().Id);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async void GetListAsyncReturnsPagedResultWithTake(int take)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetListAsync(take: take);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(take, result.Items.Count());
    }

    [Theory]
    [InlineData((int[])[1])]
    [InlineData((int[])[2])]
    public async void GetListAsyncReturnsPagedResultWithCuisineIds(int[] cuisineIds)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetListAsync(cuisineIds: cuisineIds);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain(result.Items, x => !cuisineIds.Contains(x.Cuisine!.Id));
    }

    [Fact]
    public async void GetListAsyncReturnsPagedResultWithDetails()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetListAsync(withDetails: true);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items.First().Ingredients);
        Assert.NotNull(result.Items.First().Instructions);
    }

    [Theory]
    [InlineData(-1, null)]
    [InlineData(null, 0)]
    [InlineData(null, IRecipeService.MaxItemsPerPage + 1)]
    public async void GetListAsyncThrowsValidationExceptionWithInvalidInputs(
        int? skip,
        int? take)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act / Assert
        await Assert.ThrowsAsync<ValidationException>(() => recipeService.GetListAsync(skip, take));
    }
}
