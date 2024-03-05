using FluentValidation;
using FluentValidation.Results;
using JonathanPotts.RecipeCatalog.Application.Validation;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Validation;

public sealed class ExtensionsUnitTests
{
    [Fact]
    public void ToDictionaryReturnsPopulatedObject()
    {
        // Arrange
        InlineValidator<string> validator = [];
        validator.RuleFor(x => x).NotEmpty();

        // Act
        ValidationException ex = new([new ValidationFailure("test", "Test failed")]);
        var errors = ex.ToDictionary();

        // Assert
        Assert.Single(errors);
        Assert.Contains("test", errors.Keys);
        Assert.Single(errors["test"]);
        Assert.Equal("Test failed", errors["test"][0]);
    }
}
