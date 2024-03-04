using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Mapping;

public sealed class CuisineExtensionsUnitTests
{
    [Fact]
    public void ToCuisineDtoReturnsPopulatedObject()
    {
        // Arrange
        Cuisine cuisine = new() { Id = 1, Name = "Test" };

        // Act
        var dto = cuisine.ToCuisineDto();

        // Assert
        Assert.Equal(cuisine.Id, dto.Id);
        Assert.Equal(cuisine.Name, dto.Name);
    }

    [Fact]
    public void ToCuisineReturnsPopulatedObject()
    {
        // Arrange
        CreateUpdateCuisineDto dto = new() { Name = "Test" };

        // Act
        var cuisine = dto.ToCuisine();
        cuisine.Id = 1; // This is normally populated by EF Core

        // Assert
        Assert.Equal(dto.Name, cuisine.Name);
    }
}
