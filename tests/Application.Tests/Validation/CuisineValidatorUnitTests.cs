using FluentValidation.TestHelper;
using JonathanPotts.RecipeCatalog.Application.Validation;
using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Validation;

public sealed class CuisineValidatorUnitTests
{
    private readonly CuisineValidator _validator = new();

    [Fact]
    public void TestValidateSucceedsWhenValid()
    {
        // Arrange
        Cuisine cuisine = new()
        {
            Id = 1,
            Name = "Test"
        };

        // Act
        var results = _validator.TestValidate(cuisine);

        // Assert
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(default(int), "Test")]
    [InlineData(1, null)]
    [InlineData(1, "")]
    public void TestValidateThrowsWhenInvalid(
        int id,
        string? name)
    {
        // Arrange
        Cuisine cuisine = new()
        {
            Id = id,
            Name = name
        };

        // Act
        var results = _validator.TestValidate(cuisine);

        // Arrange
        results.ShouldHaveAnyValidationError();
    }
}
