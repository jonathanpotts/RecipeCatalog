using System.Security.Claims;
using IdGen;
using IdGen.DependencyInjection;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Domain.Data;
using JonathanPotts.RecipeCatalog.Domain.Models;
using JonathanPotts.RecipeCatalog.WebApi.Apis;
using JonathanPotts.RecipeCatalog.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.WebApi.Tests.Apis;

public sealed class RecipesApiUnitTests : IDisposable
{
    private readonly ClaimsPrincipal _adminUser;
    private readonly SqliteConnection _connection;
    private readonly RecipesApi.Services _services;
    private readonly ClaimsPrincipal _user;

    public RecipesApiUnitTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        ApplicationDbContext context = new(contextOptions);
        context.Database.EnsureCreated();

        context.Users.AddRange(TestData.Users);
        context.Roles.AddRange(TestData.Roles);
        context.UserRoles.AddRange(TestData.UserRoles);
        context.Cuisines.AddRange(TestData.Cuisines);
        context.Recipes.AddRange(TestData.Recipes);
        context.SaveChanges();

        ServiceCollection serviceCollection = new();

        serviceCollection.AddSingleton(context);

        serviceCollection.AddAuthorization();
        serviceCollection.AddScoped<IAuthorizationHandler, RecipeAuthorizationHandler>();

        serviceCollection.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        serviceCollection.AddIdGen(0, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var idGenerator = serviceProvider.GetRequiredService<IdGenerator>();
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        _services = new RecipesApi.Services(context, idGenerator, authorizationService, userManager);

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
        _services.Context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async void GetListAsyncReturnsRecipes()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_services, null, null, null, null);

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
            () => Assert.Null(firstItem.Description),
            () => Assert.Equal(new DateTime(638412047602332665, DateTimeKind.Utc), firstItem.Created),
            () => Assert.Null(firstItem.Modified),
            () => Assert.Null(firstItem.Ingredients),
            () => Assert.Null(firstItem.Instructions));
    }

    [Fact]
    public async void GetListAsyncReturnsRecipesForValidTop()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_services, 3, null, null, null);

        // Assert
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
        var okResult = (Ok<PagedResult<RecipeWithCuisineDto>>)result.Result;
        Assert.NotNull(okResult.Value);

        Assert.Equal(3, okResult.Value.Items.Count());
    }

    [Fact]
    public async void GetListAsyncReturnsValidationProblemForInvalidTop()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_services, 0, null, null, null);

        // Assert
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async void GetListAsyncReturnsRecipesForLast()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_services, null, 6462258523668480, null, null);

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
        var result = await RecipesApi.GetListAsync(_services, null, null, [4], null);

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
        var result = await RecipesApi.GetListAsync(_services, null, null, null, true);

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
        var result = await RecipesApi.GetAsync(_services, 6462258523668480);

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
        var result = await RecipesApi.GetAsync(_services, 123);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async void PostAsyncReturnsRecipeForValidModel()
    {
        // Arrange
        RecipeCreateOrUpdateDto newRecipe = new()
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
        var currentId = _services.IdGenerator.CreateId();

        // Act
        var result = await RecipesApi.PostAsync(_services, _adminUser, newRecipe);

        // Assert
        Assert.IsType<Created<RecipeWithCuisineDto>>(result.Result);
        var createdResult = (Created<RecipeWithCuisineDto>)result.Result;
        Assert.NotNull(createdResult.Value);

        Assert.Equal($"/api/v1/recipes/{createdResult.Value.Id}", createdResult.Location);

        Assert.Multiple(
            () => Assert.True(currentId < createdResult.Value.Id),
            () => Assert.Equal(_services.UserManager.GetUserId(_adminUser), createdResult.Value.OwnerId),
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
        RecipeCreateOrUpdateDto newRecipe = new()
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
        var result = await RecipesApi.PostAsync(_services, new ClaimsPrincipal(), newRecipe);

        // Assert
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async void PutAsyncReturnsArticleForValidModel()
    {
        // Arrange
        RecipeCreateOrUpdateDto updatedRecipe = new()
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
        var result = await RecipesApi.PutAsync(_services, _adminUser, 6462416804118528, updatedRecipe);

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
        RecipeCreateOrUpdateDto updatedRecipe = new()
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
        var result = await RecipesApi.PutAsync(_services, _adminUser, 123, updatedRecipe);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async void PutAsyncReturnsArticleForNonOwnerAdministratorUser()
    {
        // Arrange
        RecipeCreateOrUpdateDto updatedRecipe = new()
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
        var result = await RecipesApi.PutAsync(_services, _adminUser, 6462160192405504, updatedRecipe);

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
        RecipeCreateOrUpdateDto updatedRecipe = new()
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
        var result = await RecipesApi.PutAsync(_services, _user, 6462416804118528, updatedRecipe);

        // Assert
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsNoContentForValidId()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_services, _adminUser, 6461870173061120);

        // Assert
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsNotFoundForInvalidId()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_services, _adminUser, 123);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsNoContentForNonOwnerAdministratorUser()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_services, _adminUser, 6462318867120128);

        // Assert
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async void DeleteAsyncReturnsForbidForNonOwnerNonAdministratorUser()
    {
        // Act
        var result = await RecipesApi.DeleteAsync(_services, _user, 6462416804118528);

        // Assert
        Assert.IsType<ForbidHttpResult>(result.Result);
    }
}
