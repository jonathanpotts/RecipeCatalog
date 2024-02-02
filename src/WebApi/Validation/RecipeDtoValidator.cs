using FluentValidation;
using JonathanPotts.RecipeCatalog.WebApi.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class RecipeDtoValidator : AbstractValidator<RecipeDto>
{
    public RecipeDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
