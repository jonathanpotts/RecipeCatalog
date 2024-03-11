using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class CreateUpdateRecipeDtoValidator : AbstractValidator<CreateUpdateRecipeDto>
{
    public CreateUpdateRecipeDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.CuisineId).NotEmpty();
        RuleFor(x => x.Ingredients).NotEmpty();
        RuleForEach(x => x.Ingredients).NotEmpty();
        RuleFor(x => x.Instructions).NotEmpty();
    }
}
