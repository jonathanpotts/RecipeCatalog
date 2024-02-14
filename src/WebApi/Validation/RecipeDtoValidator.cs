using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

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
