using FluentValidation.TestHelper;
using RecipeCatalog.Application.Contracts.Models;
using RecipeCatalog.Application.Validation;

namespace RecipeCatalog.Application.Tests.Validation;

public sealed class CreateUpdateCuisineDtoValidatorUnitTests
{
    private readonly CreateUpdateCuisineDtoValidator _validator = new();

    [Fact]
    public void TestValidateSucceedsWhenValid()
    {
        // Arrange
        CreateUpdateCuisineDto dto = new()
        {
            Name = "Test"
        };

        // Act
        var results = _validator.TestValidate(dto);

        // Assert
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TestValidateThrowsWhenInvalid(
        string? name)
    {
        // Arrange
        CreateUpdateCuisineDto dto = new()
        {
            Name = name
        };

        // Act
        var results = _validator.TestValidate(dto);

        // Arrange
        results.ShouldHaveAnyValidationError();
    }
}
