using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class RecipeWithCuisineDtoValidator : AbstractValidator<RecipeWithCuisineDto>
{
    public RecipeWithCuisineDtoValidator()
    {
        Include(new RecipeDtoValidator());

        RuleFor(x => x.Cuisine).NotEmpty();
    }
}
