using FluentValidation.TestHelper;
using RecipeCatalog.Application.Contracts.Models;
using RecipeCatalog.Application.Validation;

namespace RecipeCatalog.Application.Tests.Validation;

public sealed class RecipeWithCuisineDtoValidatorUnitTests
{
    private readonly RecipeWithCuisineDtoValidator _validator = new();

    [Fact]
    public void TestValidateSucceedsWhenValid()
    {
        // Arrange
        RecipeWithCuisineDto recipe = new()
        {
            Id = 6461870173061120,
            OwnerId = "d7df5331-1c53-491f-8b71-91989846874f",
            Name = "Test",
            Cuisine = new CuisineDto { Id = 1, Name = "Test" },
            Created = new DateTime(638412046299055561, DateTimeKind.Utc),
            Ingredients = ["Ingredient 1"],
            Instructions = new()
            {
                Markdown = "This is a test.",
                Html = "<p>This is a test.</p>\n"
            }
        };

        // Act
        var results = _validator.TestValidate(recipe);

        // Assert
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(default(long), "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, null, "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", null, 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "", 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", default(int), (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, null, 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])[], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])[""], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], null, "This is a test.", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, null, "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "", "<p>This is a test.</p>\n", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", null, null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, null, null, null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, null, "", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "", null, null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "", "", null)]
    [InlineData(6461870173061120, "d7df5331-1c53-491f-8b71-91989846874f", "Test", 1, (string[])["Ingredient 1"], 638412046299055561, "This is a test.", "<p>This is a test.</p>\n", "")]
    public void TestValidateThrowsWhenInvalid(
        long id,
        string? ownerId,
        string? name,
        int cuisineId,
        string[]? ingredients,
        long? created,
        string? instructionsMarkdown,
        string? instructionsHtml,
        string? coverImageUrl)
    {
        // Arrange
        RecipeWithCuisineDto recipe = new()
        {
            Id = id,
            OwnerId = ownerId,
            Name = name,
            Cuisine = new CuisineDto { Id = cuisineId },
            Ingredients = ingredients,
        };

        if (created.HasValue)
        {
            recipe.Created = new DateTime(created.Value, DateTimeKind.Utc);
        }

        if (instructionsMarkdown != null)
        {
            recipe.Instructions ??= new();
            recipe.Instructions.Markdown = instructionsMarkdown;
        }

        if (instructionsHtml != null)
        {
            recipe.Instructions ??= new();
            recipe.Instructions.Html = instructionsHtml;
        }

        if (coverImageUrl != null)
        {
            recipe.CoverImage ??= new();
            recipe.CoverImage.Url = coverImageUrl;
        }

        // Act
        var results = _validator.TestValidate(recipe);

        // Arrange
        results.ShouldHaveAnyValidationError();
    }
}
