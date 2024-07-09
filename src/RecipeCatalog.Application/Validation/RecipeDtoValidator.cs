using FluentValidation;
using RecipeCatalog.Application.Contracts.Models;

namespace RecipeCatalog.Application.Validation;

public class RecipeDtoValidator : AbstractValidator<RecipeDto>
{
    public RecipeDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Created).NotEmpty();

        When(x => x.CoverImage != null, () =>
        {
            RuleFor(x => x.CoverImage!).SetValidator(new ImageDataValidator());
        });

        When(x => x.Instructions != null, () =>
        {
            RuleFor(x => x.Instructions!).SetValidator(new MarkdownDataValidator());
        });
    }
}
