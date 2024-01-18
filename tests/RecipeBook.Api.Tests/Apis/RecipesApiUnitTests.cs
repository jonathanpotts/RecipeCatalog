using IdGen;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Apis;
using RecipeBook.Api.Data;
using RecipeBook.Api.Models;

namespace RecipeBook.Api.Tests.Apis;

public sealed class RecipesApiUnitTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly RecipesApi.Services _services;

    public RecipesApiUnitTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        ApplicationDbContext context = new(contextOptions);
        context.Database.Migrate();

        //context.Recipes.AddRange(RecipesApiTestData.Recipes);
        //context.SaveChanges();

        var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        IdGenerator idGenerator = new(0, new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

        _services = new RecipesApi.Services(context, idGenerator);
    }

    public void Dispose()
    {
        _services.Context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async void GetListAsync_ReturnsRecipes()
    {
        // Act
        var result = await RecipesApi.GetListAsync(_services, null, null);

        // Assert
        var okResult = (Ok<PagedResult<Recipe>>)result.Result;
        Assert.NotNull(okResult);

        Assert.NotNull(okResult.Value);
    }
}
