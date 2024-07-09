using IdGen;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeCatalog.Domain;
using RecipeCatalog.Domain.Entities;
using RecipeCatalog.WebApi.Shared.Data;

namespace RecipeCatalog.WebApi.Shared.Tests.Data;

public sealed class DbMigratorUnitTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void MigrateAddsSeedData(bool adminPasswordDefined)
    {
        // Arrange
        Dictionary<string, string?> configurationValues = [];

        if (adminPasswordDefined)
        {
            configurationValues.Add("AdminPassword", "TestPassword123!");
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        using SqliteConnection connection = new("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<RecipeCatalogDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new RecipeCatalogDbContext(contextOptions);

        IdGenerator idGenerator = new(0);

        ServiceCollection services = new();
        services.AddSingleton(context);
        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<RecipeCatalogDbContext>();

        var serviceProvider = services.BuildServiceProvider();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        DbMigrator dbMigrator = new(configuration, context, idGenerator, userManager, roleManager);

        // Act
        dbMigrator.Migrate();

        // Assert
        Assert.True(context.Users.Any());
        Assert.True(context.Roles.Any());
        Assert.True(context.UserRoles.Any());
        Assert.True(context.Cuisines.Any());
        Assert.True(context.Recipes.Any());
    }
}
