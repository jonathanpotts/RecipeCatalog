using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class RecipeWithCuisineDtoValidator : AbstractValidator<RecipeWithCuisineDto>
{
    public RecipeWithCuisineDtoValidator()
    {
        RuleFor(x => x.Cuisine).NotEmpty();
    }
}
