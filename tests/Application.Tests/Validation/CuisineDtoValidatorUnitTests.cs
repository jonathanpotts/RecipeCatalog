using FluentValidation.TestHelper;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Validation;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Validation;

public sealed class CuisineDtoValidatorUnitTests
{
    private readonly CuisineDtoValidator _validator = new();

    [Fact]
    public void TestValidateSucceedsWhenValid()
    {
        // Arrange
        CuisineDto cuisine = new()
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
        CuisineDto cuisine = new()
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
