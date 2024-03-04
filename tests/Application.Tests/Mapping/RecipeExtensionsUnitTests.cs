using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Mapping;

public sealed class RecipeExtensionsUnitTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToRecipeWithCuisineDtoReturnsPopulatedObject(bool withDetails)
    {
        // Arrange
        Recipe recipe = new()
        {
            Id = 6461870173061120,
            OwnerId = "d7df5331-1c53-491f-8b71-91989846874f",
            Name = "Test",
            CuisineId = 1,
            Cuisine = new Cuisine { Id = 1, Name = "Test" },
            Created = new DateTime(638412046299055561, DateTimeKind.Utc),
            Modified = new DateTime(638451812957543811, DateTimeKind.Utc),
            Ingredients = ["Ingredient 1"],
            Instructions = new()
            {
                Markdown = "This is a test.",
                Html = "<p>This is a test.</p>\n"
            }
        };

        // Act
        var dto = recipe.ToRecipeWithCuisineDto(withDetails);

        // Assert
        Assert.Equal(recipe.Id, dto.Id);
        Assert.Equal(recipe.OwnerId, dto.OwnerId);
        Assert.Equal(recipe.Name, dto.Name);
        Assert.Equal(recipe.Cuisine?.Id, dto.Cuisine?.Id);
        Assert.Equal(recipe.Cuisine?.Name, dto.Cuisine?.Name);
        Assert.Equal(recipe.Created, dto.Created);
        Assert.Equal(recipe.Modified, dto.Modified);

        if (withDetails)
        {
            Assert.Equal(recipe.Ingredients, dto.Ingredients);
            Assert.Equal(recipe.Instructions?.Markdown, dto.Instructions?.Markdown);
            Assert.Equal(recipe.Instructions?.Html, dto.Instructions?.Html);
        }
        else
        {
            Assert.Null(dto.Ingredients);
            Assert.Null(dto.Instructions);
        }
    }

    [Fact]
    public void ToRecipeReturnsPopulatedObject()
    {
        // Arrange
        CreateUpdateRecipeDto dto = new()
        {
            Name = "Test",
            CuisineId = 1,
            Ingredients = ["Ingredient 1"],
            Instructions = "This is a test."
        };

        // Act
        var recipe = dto.ToRecipe();

        // Assert
        Assert.Equal(recipe.Name, dto.Name);
        Assert.Equal(dto.CuisineId, recipe.CuisineId);
        Assert.Equal(dto.Ingredients, recipe.Ingredients);
        Assert.Equal(dto.Instructions, recipe.Instructions?.Markdown);
    }
}
