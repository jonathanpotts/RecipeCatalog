using FluentValidation.TestHelper;
using RecipeCatalog.Application.Validation;
using RecipeCatalog.Domain.Shared.ValueObjects;

namespace RecipeCatalog.Application.Tests.Validation;

public sealed class MarkdownDataValidatorUnitTests
{
    private readonly MarkdownDataValidator _validator = new();

    [Fact]
    public void TestValidateSucceedsWhenValid()
    {
        // Arrange
        MarkdownData markdownData = new()
        {
            Markdown = "This is a test.",
            Html = "<p>This is a test.</p>\n"
        };

        // Act
        var results = _validator.TestValidate(markdownData);

        // Assert
        results.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null, "<p>This is a test.</p>\n")]
    [InlineData("", "<p>This is a test.</p>\n")]
    [InlineData("This is a test.", null)]
    [InlineData("This is a test.", "")]
    public void TestValidateThrowsWhenInvalid(
        string? markdown,
        string? html)
    {
        // Arrange
        MarkdownData markdownData = new()
        {
            Markdown = markdown,
            Html = html
        };

        // Act
        var results = _validator.TestValidate(markdownData);

        // Arrange
        results.ShouldHaveAnyValidationError();
    }
}
