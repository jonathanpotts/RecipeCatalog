using FluentValidation.TestHelper;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Validation;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Validation;

public sealed class CreateUpdateRecipeDtoValidatorUnitTests
{
    private readonly CreateUpdateRecipeDtoValidator _validator = new();

    [Fact]
    public void TestValidateSucceedsWhenValid()
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
        var results = _validator.TestValidate(dto);

        // Assert
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null, 1, (string[])["Ingredient 1"], "This is a test.")]
    [InlineData("", 1, (string[])["Ingredient 1"], "This is a test.")]
    [InlineData("Test", default(int), (string[])["Ingredient 1"], "This is a test.")]
    [InlineData("Test", 1, null, "This is a test.")]
    [InlineData("Test", 1, (string[])[], "This is a test.")]
    [InlineData("Test", 1, (string[])[""], "This is a test.")]
    [InlineData("Test", 1, (string[])["Ingredient 1"], null)]
    [InlineData("Test", 1, (string[])["Ingredient 1"], "")]
    public void TestValidateThrowsWhenInvalid(
        string? name,
        int cuisineId,
        string[]? ingredients,
        string? instructions)
    {
        // Arrange
        CreateUpdateRecipeDto dto = new()
        {
            Name = name,
            CuisineId = cuisineId,
            Ingredients = ingredients,
            Instructions = instructions
        };

        // Act
        var results = _validator.TestValidate(dto);

        // Arrange
        results.ShouldHaveAnyValidationError();
    }
}
