using System.Security.Claims;
using IdGen;
using IdGen.DependencyInjection;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Services;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.WebApi.Shared.Apis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.WebApi.Tests.Apis;

public sealed class RecipesApiUnitTests : IDisposable
{

    private readonly SqliteConnection _connection;
    private readonly RecipeCatalogDbContext _context;
    private readonly RecipeService _recipeService;
    private readonly ClaimsPrincipal _adminUser;
    private readonly ClaimsPrincipal _user;

    public RecipesApiUnitTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var contextOptions = new DbContextOptionsBuilder<RecipeCatalogDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new(contextOptions);
        _context.Database.EnsureCreated();

        _context.Users.AddRange(TestData.Users);
        _context.Roles.AddRange(TestData.Roles);
        _context.UserRoles.AddRange(TestData.UserRoles);
        _context.Cuisines.AddRange(TestData.Cuisines);
        _context.Recipes.AddRange(TestData.Recipes);
        _context.SaveChanges();

        ServiceCollection serviceCollection = new();

        serviceCollection.AddSingleton(_context);

        serviceCollection.AddAuthorization();
        serviceCollection.AddScoped<IAuthorizationHandler, RecipeAuthorizationHandler>();

        serviceCollection.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<RecipeCatalogDbContext>();

        var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        serviceCollection.AddIdGen(0, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var idGenerator = serviceProvider.GetRequiredService<IdGenerator>();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        _recipeService = new(_context, idGenerator, userManager, authorizationService, serviceProvider);

        _adminUser = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, "73edf737-df51-4c06-ac6f-3ec6d79f1f12")
        ], "Test"));

        _user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, "d7df5331-1c53-491f-8b71-91989846874f")
        ], "Test"));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async void GetListAsyncReturnsRecipes()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_recipeService, null, null, null, null, default);

        // Assert
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
        var okResult = (Ok<PagedResult<RecipeWithCuisineDto>>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.NotEmpty(okResult.Value.Items);

        var firstItem = okResult.Value.Items.First();

        Assert.Multiple(
            () => Assert.Equal(6462416804118528, firstItem.Id),
            () => Assert.Equal("73edf737-df51-4c06-ac6f-3ec6d79f1f12", firstItem.OwnerId),
            () => Assert.Equal("Test Recipe 5", firstItem.Name),
            () =>
            {
                Assert.NotNull(firstItem.CoverImage);
                Assert.Equal("/api/v1/recipes/6462416804118528/coverImage", firstItem.CoverImage.Url);
                Assert.Equal("A photo of test recipe 5", firstItem.CoverImage.AltText);
            },
            () =>
            {
                Assert.NotNull(firstItem.Cuisine);
                Assert.Equal(1, firstItem.Cuisine.Id);
                Assert.Equal("Test", firstItem.Cuisine.Name);
            },
            () => Assert.Equal("This is a test.", firstItem.Description),
            () => Assert.Equal(new DateTime(638412047602332665, DateTimeKind.Utc), firstItem.Created),
            () => Assert.Null(firstItem.Modified),
            () => Assert.Null(firstItem.Ingredients),
            () => Assert.Null(firstItem.Instructions));
    }

    [Fact]
    public async void GetListAsyncReturnsRecipesForValidTake()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_recipeService, null, 3, null, null, default);

        // Assert
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
        var okResult = (Ok<PagedResult<RecipeWithCuisineDto>>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.Equal(3, okResult.Value.Items.Count());
    }

    [Fact]
    public async void GetListAsyncReturnsValidationProblemForInvalidTake()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_recipeService, null, 0, null, null, default);

        // Assert
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async void GetListAsyncReturnsRecipesForSkip()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_recipeService, 3, null, null, null, default);

        // Assert
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
        var okResult = (Ok<PagedResult<RecipeWithCuisineDto>>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.Multiple(
            () => Assert.NotEmpty(okResult.Value.Items),
            () => Assert.DoesNotContain(okResult.Value.Items,
                x => x.Id >= 6462258523668480),
            () => Assert.Equal(6462160192405504, okResult.Value.Items.FirstOrDefault()?.Id));
    }

    [Fact]
    public async void GetListAsyncReturnsRecipesForCuisineIds()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_recipeService, null, null, [4], null, default);

        // Assert
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
        var okResult = (Ok<PagedResult<RecipeWithCuisineDto>>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.Multiple(
            () => Assert.NotEmpty(okResult.Value.Items),
            () => Assert.All(okResult.Value.Items, x => Assert.Equal(4, x.Cuisine?.Id)));
    }

    [Fact]
    public async void GetListAsyncReturnsRecipesForWithDetails()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_recipeService, null, null, null, true, default);

        // Assert
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
        var okResult = (Ok<PagedResult<RecipeWithCuisineDto>>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.NotEmpty(okResult.Value.Items);

        var firstItem = okResult.Value.Items.First();

        Assert.Multiple(
            () => Assert.Equal(6462416804118528, firstItem.Id),
            () => Assert.Equal("73edf737-df51-4c06-ac6f-3ec6d79f1f12", firstItem.OwnerId),
            () => Assert.Equal("Test Recipe 5", firstItem.Name),
            () =>
            {
                Assert.NotNull(firstItem.CoverImage);
                Assert.Equal("/api/v1/recipes/6462416804118528/coverImage", firstItem.CoverImage.Url);
                Assert.Equal("A photo of test recipe 5", firstItem.CoverImage.AltText);
            },
            () =>
            {
                Assert.NotNull(firstItem.Cuisine);
                Assert.Equal(1, firstItem.Cuisine.Id);
                Assert.Equal("Test", firstItem.Cuisine.Name);
            },
            () => Assert.Equal("This is a test.", firstItem.Description),
            () => Assert.Equal(new DateTime(638412047602332665, DateTimeKind.Utc), firstItem.Created),
            () => Assert.Null(firstItem.Modified),
            () =>
            {
                Assert.NotNull(firstItem.Ingredients);
                Assert.Collection(firstItem.Ingredients,
                    x => Assert.Equal("1 tsp of test ingredient 1", x),
                    x => Assert.Equal("1 cup of test ingredient 2", x));
            },
            () => Assert.Equal("This is a test.", firstItem.Instructions?.Markdown),
            () => Assert.Equal("<p>This is a test.</p>\n", firstItem.Instructions?.Html));
    }

    [Fact]
    public async void GetAsyncReturnsRecipeForValidId()
    {
        // Act
        var result = await RecipesApi.GetAsync(_recipeService, 6462258523668480, default);

        // Assert
        Assert.IsType<Ok<RecipeWithCuisineDto>>(result.Result);
        var okResult = (Ok<RecipeWithCuisineDto>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.Multiple(
            () => Assert.Equal(6462258523668480, okResult.Value.Id),
            () => Assert.Equal("73edf737-df51-4c06-ac6f-3ec6d79f1f12", okResult.Value.OwnerId),
            () => Assert.Equal("Test Recipe 3", okResult.Value.Name),
            () =>
            {
                Assert.NotNull(okResult.Value.CoverImage);
                Assert.Equal("/api/v1/recipes/6462258523668480/coverImage", okResult.Value.CoverImage.Url);
                Assert.Equal("A photo of test recipe 3", okResult.Value.CoverImage.AltText);
            },
            () =>
            {
                Assert.NotNull(okResult.Value.Cuisine);
                Assert.Equal(1, okResult.Value.Cuisine.Id);
                Assert.Equal("Test", okResult.Value.Cuisine.Name);
            },
            () => Assert.Equal("This is a test.", okResult.Value.Description),
            () => Assert.Equal(new DateTime(638412047224957774, DateTimeKind.Utc), okResult.Value.Created),
            () => Assert.Null(okResult.Value.Modified),
            () =>
            {
                Assert.NotNull(okResult.Value.Ingredients);
                Assert.Collection(okResult.Value.Ingredients,
                    x => Assert.Equal("1 tsp of test ingredient 1", x),
                    x => Assert.Equal("1 cup of test ingredient 2", x));
            },
            () => Assert.Equal("This is a test.", okResult.Value.Instructions?.Markdown),
            () => Assert.Equal("<p>This is a test.</p>\n", okResult.Value.Instructions?.Html));
    }

    [Fact]
    public async void GetAsyncReturnsNotFoundForInvalidId()
    {
        // Act
        var result = await RecipesApi.GetAsync(_recipeService, 123, default);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async void PostAsyncReturnsRecipeForValidModel()
    {
        // Arrange
        CreateUpdateRecipeDto newRecipe = new()
        {
            Name = "New Recipe",
            CuisineId = 2,
            Description = "This is a new recipe.",
            Ingredients =
            [
                "1 lb of new ingredient 1",
                "3 oz of new ingredient 2"
            ],
            Instructions = "This is new."
        };

        var utcNow = DateTime.UtcNow;

        // Act
        var result = await RecipesApi.PostAsync(_recipeService, newRecipe, _adminUser, default);

        // Assert
        Assert.IsType<Created<RecipeWithCuisineDto>>(result.Result);
        var createdResult = (Created<RecipeWithCuisineDto>)result.Result;
        Assert.NotNull(createdResult.Value);

        Assert.Equal($"/api/v1/recipes/{createdResult.Value.Id}", createdResult.Location);

        Assert.Multiple(
            () => Assert.True(0 < createdResult.Value.Id),
            () => Assert.Equal("73edf737-df51-4c06-ac6f-3ec6d79f1f12", createdResult.Value.OwnerId),
            () => Assert.Equal(newRecipe.Name, createdResult.Value.Name),
            () =>
            {
                Assert.NotNull(createdResult.Value.Cuisine);
                Assert.Equal(2, createdResult.Value.Cuisine.Id);
                Assert.Equal("New", createdResult.Value.Cuisine.Name);
            },
            () => Assert.Equal(newRecipe.Description, createdResult.Value.Description),
            () => Assert.True(utcNow < createdResult.Value.Created),
            () => Assert.Null(createdResult.Value.Modified),
            () =>
            {
                Assert.NotNull(createdResult.Value.Ingredients);
                Assert.Collection(createdResult.Value.Ingredients,
                    x => Assert.Equal("1 lb of new ingredient 1", x),
                    x => Assert.Equal("3 oz of new ingredient 2", x));
            },
            () => Assert.Equal(newRecipe.Instructions, createdResult.Value.Instructions?.Markdown),
            () => Assert.Equal("<p>This is new.</p>\n", createdResult.Value.Instructions?.Html));
    }

    [Fact]
    public async void PostAsyncReturnsForbidForAnonymousUser()
    {
        // Arrange
        CreateUpdateRecipeDto newRecipe = new()
        {
            Name = "New Recipe",
            CuisineId = 2,
            Description = "This is a new recipe.",
            Ingredients =
            [
                "1 lb of new ingredient 1",
                "3 oz of new ingredient 2"
            ],
            Instructions = "This is new."
        };

        // Act
        var result = await RecipesApi.PostAsync(_recipeService, newRecipe, new ClaimsPrincipal(), default);

        // Assert
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async void PutAsyncReturnsArticleForValidModel()
    {
        // Arrange
        CreateUpdateRecipeDto updatedRecipe = new()
        {
            Name = "Updated Recipe",
            CuisineId = 3,
            Description = "This is an updated recipe.",
            Ingredients =
            [
                "2 tbsp of updated ingredient 1",
                "3 updated ingredient 2"
            ],
            Instructions = "This is updated."
        };

        var utcNow = DateTime.UtcNow;

        // Act
        var result = await RecipesApi.PutAsync(_recipeService, 6462416804118528, updatedRecipe, _adminUser, default);

        // Assert
        Assert.IsType<Ok<RecipeWithCuisineDto>>(result.Result);
        var okResult = (Ok<RecipeWithCuisineDto>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.Multiple(
            () => Assert.Equal(6462416804118528, okResult.Value.Id),
            () => Assert.Equal("73edf737-df51-4c06-ac6f-3ec6d79f1f12", okResult.Value.OwnerId),
            () => Assert.Equal(updatedRecipe.Name, okResult.Value.Name),
            () =>
            {
                Assert.NotNull(okResult.Value.Cuisine);
                Assert.Equal(3, okResult.Value.Cuisine.Id);
                Assert.Equal("Updated", okResult.Value.Cuisine.Name);
            },
            () => Assert.Equal(updatedRecipe.Description, okResult.Value.Description),
            () => Assert.Equal(new DateTime(638412047602332665, DateTimeKind.Utc), okResult.Value.Created),
            () => Assert.NotNull(okResult.Value.Modified),
            () => Assert.True(utcNow < okResult.Value.Modified),
            () =>
            {
                Assert.NotNull(okResult.Value.Ingredients);
                Assert.Collection(okResult.Value.Ingredients,
                    x => Assert.Equal("2 tbsp of updated ingredient 1", x),
                    x => Assert.Equal("3 updated ingredient 2", x));
            },
            () => Assert.Equal(updatedRecipe.Instructions, okResult.Value.Instructions?.Markdown),
            () => Assert.Equal("<p>This is updated.</p>\n", okResult.Value.Instructions?.Html));
    }

    [Fact]
    public async void PutAsyncReturnsNotFoundForInvalidId()
    {
        // Arrange
        CreateUpdateRecipeDto updatedRecipe = new()
        {
            Name = "Updated Recipe",
            CuisineId = 3,
            Description = "This is an updated recipe.",
            Ingredients =
            [
                "2 tbsp of updated ingredient 1",
                "3 updated ingredient 2"
            ],
            Instructions = "This is updated."
        };

        // Act
        var result = await RecipesApi.PutAsync(_recipeService, 123, updatedRecipe, _adminUser, default);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async void PutAsyncReturnsArticleForNonOwnerAdministratorUser()
    {
        // Arrange
        CreateUpdateRecipeDto updatedRecipe = new()
        {
            Name = "Updated Recipe",
            CuisineId = 3,
            Description = "This is an updated recipe.",
            Ingredients =
            [
                "2 tbsp of updated ingredient 1",
                "3 updated ingredient 2"
            ],
            Instructions = "This is updated."
        };

        var utcNow = DateTime.UtcNow;

        // Act
        var result = await RecipesApi.PutAsync(_recipeService, 6462160192405504, updatedRecipe, _adminUser, default);

        // Assert
        Assert.IsType<Ok<RecipeWithCuisineDto>>(result.Result);
        var okResult = (Ok<RecipeWithCuisineDto>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.Multiple(
            () => Assert.Equal(6462160192405504, okResult.Value.Id),
            () => Assert.Equal("d7df5331-1c53-491f-8b71-91989846874f", okResult.Value.OwnerId),
            () => Assert.Equal(updatedRecipe.Name, okResult.Value.Name),
            () =>
            {
                Assert.NotNull(okResult.Value.Cuisine);
                Assert.Equal(3, okResult.Value.Cuisine.Id);
                Assert.Equal("Updated", okResult.Value.Cuisine.Name);
            },
            () => Assert.Equal(updatedRecipe.Description, okResult.Value.Description),
            () => Assert.Equal(new DateTime(638412046990521543, DateTimeKind.Utc), okResult.Value.Created),
            () => Assert.NotNull(okResult.Value.Modified),
            () => Assert.True(utcNow < okResult.Value.Modified),
            () =>
            {
                Assert.NotNull(okResult.Value.Ingredients);
                Assert.Collection(okResult.Value.Ingredients,
                    x => Assert.Equal("2 tbsp of updated ingredient 1", x),
                    x => Assert.Equal("3 updated ingredient 2", x));
            },
            () => Assert.Equal(updatedRecipe.Instructions, okResult.Value.Instructions?.Markdown),
            () => Assert.Equal("<p>This is updated.</p>\n", okResult.Value.Instructions?.Html));
    }

    [Fact]
    public async void PutAsyncReturnsForbidForNonOwnerNonAdministratorUser()
    {
        // Arrange
        CreateUpdateRecipeDto updatedRecipe = new()
        {
            Name = "Updated Recipe",
            CuisineId = 3,
            Description = "This is an updated recipe.",
            Ingredients =
            [
                "2 tbsp of updated ingredient 1",
                "3 updated ingredient 2"
            ],
            Instructions = "This is updated."
        };

        // Act
        var result = await RecipesApi.PutAsync(_recipeService, 6462416804118528, updatedRecipe, _user, default);

        // Assert
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsNoContentForValidId()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_recipeService, 6461870173061120, _adminUser, default);

        // Assert
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsNotFoundForInvalidId()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_recipeService, 123, _adminUser, default);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsNoContentForNonOwnerAdministratorUser()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_recipeService, 6462318867120128, _adminUser, default);

        // Assert
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsForbidForNonOwnerNonAdministratorUser()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_recipeService, 6462416804118528, _user, default);

        // Assert
        Assert.IsType<ForbidHttpResult>(result.Result);
    }
}
