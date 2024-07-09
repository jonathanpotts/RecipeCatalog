using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RecipeCatalog.Domain.Entities;

namespace RecipeCatalog.Domain.Tests;

public sealed class ExtensionsUnitTests
{
    [Fact]
    public void AddDomainServicesReturnsPopulatedServiceProvider()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddDomainServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<RecipeCatalogDbContext>());
    }

    [Fact]
    public void AddDomainStoresReturnsPopulatedIdentityBuilder()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddDomainServices();

        services.AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddDomainStores();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IUserStore<User>>());
    }
}
