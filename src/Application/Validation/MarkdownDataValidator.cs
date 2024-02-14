using FluentValidation;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class MarkdownDataValidator : AbstractValidator<MarkdownData>
{
    public MarkdownDataValidator()
    {
        RuleFor(x => x.Markdown).NotEmpty();
        RuleFor(x => x.Html).NotEmpty();
    }
}
