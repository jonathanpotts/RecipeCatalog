using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Mapping;

public sealed class RecipeExtensionsUnitTests
{
    [Theory]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    public void ToRecipeWithCuisineDtoReturnsPopulatedObject(bool withCoverImage, bool withCuisine, bool withDetails)
    {
        // Arrange
        Recipe recipe = new()
        {
            Id = 6461870173061120,
            OwnerId = "d7df5331-1c53-491f-8b71-91989846874f",
            Name = "Test",
            CoverImage = withCoverImage ? new ImageData { AltText = "test" } : null,
            Cuisine = withCuisine ? new Cuisine { Id = 1, Name = "Test" } : null,
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

        if (withCoverImage)
        {
            Assert.NotNull(dto.CoverImage);
            Assert.NotNull(dto.CoverImage?.Url);
            Assert.Equal(recipe.CoverImage?.AltText, dto.CoverImage?.AltText);
        }
        else
        {
            Assert.Null(dto.CoverImage);
        }

        if (withCuisine)
        {
            Assert.NotNull(recipe.Cuisine);
            Assert.Equal(recipe.Cuisine?.Id, dto.Cuisine?.Id);
            Assert.Equal(recipe.Cuisine?.Name, dto.Cuisine?.Name);
        }
        else
        {
            Assert.Null(recipe.Cuisine);
        }

        if (withDetails)
        {
            Assert.NotNull(dto.Ingredients);
            Assert.Equal(recipe.Ingredients, dto.Ingredients);

            Assert.NotNull(dto.Instructions);
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
