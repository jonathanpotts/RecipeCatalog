using System.Security;
using System.Security.Claims;
using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Services;
using JonathanPotts.RecipeCatalog.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Services;

public sealed class CuisineServiceUnitTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<RecipeCatalogDbContext> _contextOptions;
    private readonly ClaimsPrincipal _admin = TestData.GetAdministrator();
    private readonly ClaimsPrincipal _user = TestData.GetUser();
    private readonly ClaimsPrincipal _anon = TestData.GetAnonymousUser();

    private RecipeCatalogDbContext CreateContext() => new(_contextOptions);

    private CuisineService CreateCuisineService(
        bool authorizationServiceSucceeds = true)
        => new(
            CreateContext(),
            Mocks.CreateAuthorizationServiceMock(authorizationServiceSucceeds).Object);

    public CuisineServiceUnitTests()
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
    public async void GetListAsyncReturnsItems()
    {
        // Arrange
        var cuisineService = CreateCuisineService();

        // Act
        var result = await cuisineService.GetListAsync();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public async void GetAsyncReturnsDto()
    {
        // Arrange
        var cuisineService = CreateCuisineService();

        // Act
        var result = await cuisineService.GetAsync(TestData.Cuisines[0].Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TestData.Cuisines[0].Id, result.Id);
    }

    [Fact]
    public async void CreateAsyncReturnsDto()
    {
        // Arrange
        var cuisineService = CreateCuisineService();

        CreateUpdateCuisineDto createDto = new()
        {
            Name = "Create Test"
        };

        // Act
        var result = await cuisineService.CreateAsync(createDto, _admin);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(CreateContext().Cuisines.Find(result.Id));
    }

    [Fact]
    public async void CreateAsyncThrowsDbUpdateExceptionWhenNameAlreadyExists()
    {
        // Arrange
        var cuisineService = CreateCuisineService();

        CreateUpdateCuisineDto createDto = new()
        {
            Name = TestData.Cuisines[0].Name
        };

        // Act / Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => cuisineService.CreateAsync(createDto, _admin));
    }

    [Fact]
    public async void CreateAsyncThrowsSecurityExceptionWhenUnauthorized()
    {
        // Arrange
        var cuisineService = CreateCuisineService(false);

        CreateUpdateCuisineDto createDto = new()
        {
            Name = TestData.Cuisines[0].Name
        };

        // Act / Assert
        await Assert.ThrowsAsync<SecurityException>(() => cuisineService.CreateAsync(createDto, _anon));
    }

    [Fact]
    public async void CreateAsyncThrowsValidationExceptionWithInvalidInput()
    {
        // Arrange
        var cuisineService = CreateCuisineService();

        CreateUpdateCuisineDto createDto = new();

        // Act / Assert
        await Assert.ThrowsAsync<ValidationException>(() => cuisineService.CreateAsync(createDto, _admin));
    }
}
