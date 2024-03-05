using FluentValidation.TestHelper;
using JonathanPotts.RecipeCatalog.Application.Validation;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Validation;

public sealed class ImageDataValidatorUnitTests
{
    private readonly ImageDataValidator _validator = new();

    [Fact]
    public void TestValidateSucceedsWhenValid()
    {
        // Arrange
        ImageData imageData = new()
        {
            Url = "test.webp",
            AltText = "Test"
        };

        // Act
        var results = _validator.TestValidate(imageData);

        // Assert
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TestValidateThrowsWhenInvalid(
        string? url)
    {
        // Arrange
        ImageData imageData = new()
        {
            Url = url
        };

        // Act
        var results = _validator.TestValidate(imageData);

        // Arrange
        results.ShouldHaveAnyValidationError();
    }
}
