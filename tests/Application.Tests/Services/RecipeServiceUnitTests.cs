using System.Security;
using System.Security.Claims;
using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Services;
using JonathanPotts.RecipeCatalog.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Services;

public sealed class RecipeServiceUnitTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<RecipeCatalogDbContext> _contextOptions;
    private readonly ClaimsPrincipal _admin = TestData.GetAdministrator();
    private readonly ClaimsPrincipal _user = TestData.GetUser();

    private RecipeCatalogDbContext CreateContext() => new(_contextOptions);

    private RecipeService CreateRecipeService(
        bool authorizationServiceSucceeds = true,
        bool withAITextGenerator = false)
        => new(
            CreateContext(),
            Mocks.CreateIdGeneratorMock().Object,
            Mocks.CreateUserManagerMock().Object,
            Mocks.CreateAuthorizationServiceMock(authorizationServiceSucceeds).Object,
            Mocks.CreateServiceProviderMock(withAITextGenerator).Object);

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

    [Fact]
    public async void GetAsyncReturnsDto()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetAsync(TestData.Recipes.First().Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TestData.Recipes.First().Id, result.Id);
    }

    [Fact]
    public async void GetCoverImageAsyncReturnsPath()
    {
        // Arrange
        var recipeService = CreateRecipeService();
        var recipeId = TestData.Recipes.First(x => !string.IsNullOrEmpty(x.CoverImage?.Url)).Id;

        // Act
        var result = await recipeService.GetCoverImageAsync(recipeId);

        // Assert
        Assert.False(string.IsNullOrEmpty(result));
    }

    [Fact]
    public async void GetCoverImageAsyncThrowsExceptionWithInvalidId()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act / Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => recipeService.GetCoverImageAsync(-1));
    }

    [Fact]
    public async void GetCoverImageAsyncThrowsExceptionWhenNoCoverImage()
    {
        // Arrange
        var recipeService = CreateRecipeService();
        var recipeId = TestData.Recipes.First(x => x.CoverImage == null).Id;

        // Act / Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => recipeService.GetCoverImageAsync(recipeId));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    public async void CreateAsyncReturnsDto(bool hasDescription, bool withAITextGenerator)
    {
        // Arrange
        var recipeService = CreateRecipeService(withAITextGenerator: withAITextGenerator);

        CreateUpdateRecipeDto createDto = new()
        {
            Name = "Test",
            CuisineId = 1,
            Description = hasDescription ? "This is a test." : null,
            Ingredients = ["Test ingredient 1"],
            Instructions = "This is a test."
        };

        // Act
        var result = await recipeService.CreateAsync(createDto, _admin);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(CreateContext().Recipes.Find(result.Id));
    }

    [Fact]
    public async void CreateAsyncThrowsSecurityExceptionWhenUnauthorized()
    {
        // Arrange
        var recipeService = CreateRecipeService(false);

        CreateUpdateRecipeDto createDto = new()
        {
            Name = "Test",
            CuisineId = 1,
            Ingredients = ["Test ingredient 1"],
            Instructions = "This is a test."
        };

        // Act / Assert
        await Assert.ThrowsAsync<SecurityException>(() => recipeService.CreateAsync(createDto, _admin));
    }

    [Fact]
    public async void CreateAsyncThrowsValidationExceptionWithInvalidInput()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        CreateUpdateRecipeDto createDto = new()
        {
            Name = "Test"
        };

        // Act / Assert
        await Assert.ThrowsAsync<ValidationException>(() => recipeService.CreateAsync(createDto, _admin));
    }
}
