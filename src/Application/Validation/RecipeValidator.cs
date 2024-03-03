using FluentValidation;
using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class RecipeValidator : AbstractValidator<Recipe>
{
    public RecipeValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.CuisineId).NotEmpty();
        RuleFor(x => x.Created).NotEmpty();
        RuleFor(x => x.Ingredients).NotEmpty().ForEach(x => x.NotEmpty());
        RuleFor(x => x.Instructions!).NotNull()
            .SetValidator(new MarkdownDataValidator());

        When(x => x.CoverImage != null, () =>
        {
            RuleFor(x => x.CoverImage!).SetValidator(new ImageDataValidator());
        });
    }
}
