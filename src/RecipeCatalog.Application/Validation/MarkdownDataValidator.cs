using FluentValidation;
using RecipeCatalog.Domain.Shared.ValueObjects;

namespace RecipeCatalog.Application.Validation;

public class MarkdownDataValidator : AbstractValidator<MarkdownData>
{
    public MarkdownDataValidator()
    {
        RuleFor(x => x.Markdown).NotEmpty();
        RuleFor(x => x.Html).NotEmpty();
    }
}
