using System.Security.Claims;
using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Services;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Tests.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Services;

public sealed class RecipeServiceUnitTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<RecipeCatalogDbContext> _contextOptions;
    private readonly ClaimsPrincipal _admin = TestData.GetAdministrator();
    private readonly ClaimsPrincipal _user = TestData.GetUser();
    private readonly ClaimsPrincipal _anon = TestData.GetAnonymousUser();

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
    public async Task GetListAsyncReturnsPagedResult()
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
    public async Task GetListAsyncReturnsPagedResultWithSkip(int skip)
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
    public async Task GetListAsyncReturnsPagedResultWithTake(int take)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetListAsync(take: take);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items.Count() <= take);
    }

    [Theory]
    [InlineData((int[])[1])]
    [InlineData((int[])[2])]
    public async Task GetListAsyncReturnsPagedResultWithCuisineIds(int[] cuisineIds)
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
    public async Task GetListAsyncReturnsPagedResultWithDetails()
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
    public async Task GetListAsyncThrowsValidationExceptionWithInvalidInputs(
        int? skip,
        int? take)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act / Assert
        await Assert.ThrowsAsync<ValidationException>(() => recipeService.GetListAsync(skip, take));
    }

    [Fact]
    public async Task GetAsyncReturnsDto()
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
    public async Task GetAsyncReturnsNullWithInvalidId()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetAsync(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCoverImageAsyncReturnsPath()
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
    public async Task GetCoverImageAsyncReturnsNullWithInvalidId()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.GetCoverImageAsync(-1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCoverImageAsyncReturnsNullWhenNoCoverImage()
    {
        // Arrange
        var recipeService = CreateRecipeService();
        var recipeId = TestData.Recipes.First(x => x.CoverImage == null).Id;

        // Act
        var result = await recipeService.GetCoverImageAsync(recipeId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCoverImageAsyncCompletesSuccessfully()
    {
        // Arrange
        var recipeService = CreateRecipeService();
        using var imageData = File.OpenRead(Path.Combine("Images", "new-image.webp"));

        // Act
        var result = await recipeService.UpdateCoverImageAsync(TestData.Recipes[0].Id, imageData, null, _admin);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateCoverImageAsyncReturnsFalseWithInvalidId()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.UpdateCoverImageAsync(-1, Stream.Null, null, _admin);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateCoverImageAsyncThrowsUnauthorizedAccessExceptionWhenUnauthorized()
    {
        // Arrange
        var recipeService = CreateRecipeService(false);

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => recipeService.UpdateCoverImageAsync(TestData.Recipes[0].Id, Stream.Null, null, _user));
    }

    [Fact]
    public async Task DeleteCoverImageAsyncCompletesSuccessfully()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.DeleteCoverImageAsync(TestData.Recipes[0].Id, _admin);

        // Assert
        Assert.True(result);

        var recipe = CreateContext().Recipes.Find(TestData.Recipes[0].Id);
        Assert.NotNull(recipe);
        Assert.Null(recipe.CoverImage);
    }

    [Fact]
    public async Task DeleteCoverImageAsyncReturnsFalseWithInvalidId()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.DeleteCoverImageAsync(-1, _admin);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCoverImageAsyncThrowsUnauthorizedExceptionWhenUnauthorized()
    {
        // Arrange
        var recipeService = CreateRecipeService(false);

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => recipeService.DeleteCoverImageAsync(TestData.Recipes[0].Id, _user));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    public async Task CreateAsyncReturnsDto(bool hasDescription, bool withAITextGenerator)
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
    public async Task CreateAsyncThrowsUnauthorizedExceptionWhenUnauthorized()
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
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => recipeService.CreateAsync(createDto, _anon));
    }

    [Fact]
    public async Task CreateAsyncThrowsValidationExceptionWithInvalidInput()
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

    [Theory]
    [InlineData(true, false)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(false, true)]
    public async Task UpdateAsyncReturnsDto(bool hasDescription, bool withAITextGenerator)
    {
        // Arrange
        var recipeService = CreateRecipeService(withAITextGenerator: withAITextGenerator);

        CreateUpdateRecipeDto updateDto = new()
        {
            Name = "Updated Test",
            CuisineId = 1,
            Description = hasDescription ? "This is a test." : null,
            Ingredients = ["Test ingredient 1"],
            Instructions = "This is a test."
        };

        // Act
        var result = await recipeService.UpdateAsync(TestData.Recipes[0].Id, updateDto, _admin);

        // Assert
        Assert.NotNull(result);

        var recipe = CreateContext().Recipes.Find(result.Id);

        Assert.NotNull(recipe);
        Assert.Equal(updateDto.Name, recipe.Name);
    }

    [Fact]
    public async Task UpdateAsyncReturnsNullWithInvalidId()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        CreateUpdateRecipeDto updateDto = new()
        {
            Name = "Updated Test",
            CuisineId = 1,
            Ingredients = ["Test ingredient 1"],
            Instructions = "This is a test."
        };

        // Act
        var result = await recipeService.UpdateAsync(-1, updateDto, _admin);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsyncThrowsUnauthorizedAccessExceptionWhenUnauthorized()
    {
        // Arrange
        var recipeService = CreateRecipeService(false);

        CreateUpdateRecipeDto updateDto = new()
        {
            Name = "Updated Test",
            CuisineId = 1,
            Ingredients = ["Test ingredient 1"],
            Instructions = "This is a test."
        };

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => recipeService.UpdateAsync(TestData.Recipes[0].Id, updateDto, _user));
    }

    [Fact]
    public async Task UpdateAsyncThrowsValidationExceptionWithInvalidInput()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        CreateUpdateRecipeDto updateDto = new()
        {
            Name = "Updated Test"
        };

        // Act / Assert
        await Assert.ThrowsAsync<ValidationException>(() => recipeService.UpdateAsync(TestData.Recipes[0].Id, updateDto, _admin));
    }

    [Fact]
    public async Task DeleteAsyncCompletesSuccessfully()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.DeleteAsync(TestData.Recipes[0].Id, _admin);

        // Assert
        Assert.True(result);

        Assert.Null(CreateContext().Recipes.Find(TestData.Recipes[0].Id));
    }

    [Fact]
    public async Task DeleteAsyncReturnsFalseWithInvalidId()
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.DeleteAsync(-1, _admin);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsyncThrowsUnauthorizedAccessExceptionWhenUnauthorized()
    {
        // Arrange
        var recipeService = CreateRecipeService(false);

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => recipeService.DeleteAsync(TestData.Recipes[0].Id, _user));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SearchAsyncReturnsPagedResult(bool withAITextGenerator)
    {
        // Arrange
        var recipeService = CreateRecipeService(withAITextGenerator: withAITextGenerator);

        // Act
        var result = await recipeService.SearchAsync(TestData.Recipes[0].Name!);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Total > 0);
        Assert.Contains(result.Items, x => x.Id == TestData.Recipes[0].Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task SearchAsyncReturnsPagedResultWithSkip(int skip)
    {
        // Arrange
        var recipeService = CreateRecipeService();
        var skippedIds = TestData.Recipes
            .Where(x => x.Name!.Contains("test", StringComparison.OrdinalIgnoreCase) || (x.Description?.Contains("test", StringComparison.OrdinalIgnoreCase) ?? false))
            .OrderByDescending(x => x.Id)
            .Take(skip)
            .Select(x => x.Id);

        // Act
        var result = await recipeService.SearchAsync("test", skip);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain(result.Items, x => skippedIds.Contains(x.Id));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task SearchAsyncReturnsPagedResultWithTake(int take)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act
        var result = await recipeService.SearchAsync("test", take: take);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items.Count() <= take);
    }

    [Theory]
    [InlineData(-1, null)]
    [InlineData(null, 0)]
    [InlineData(null, IRecipeService.MaxItemsPerPage + 1)]
    public async Task SearchAsyncThrowsValidationExceptionWithInvalidInput(int? skip, int? take)
    {
        // Arrange
        var recipeService = CreateRecipeService();

        // Act / Assert
        await Assert.ThrowsAsync<ValidationException>(() => recipeService.SearchAsync("test", skip, take));
    }
}
