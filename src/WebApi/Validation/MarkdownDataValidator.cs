using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class MarkdownDataValidator : AbstractValidator<MarkdownData>
{
    public MarkdownDataValidator()
    {
        RuleFor(x => x.Markdown).NotEmpty();
        RuleFor(x => x.Html).NotEmpty();
    }
}
