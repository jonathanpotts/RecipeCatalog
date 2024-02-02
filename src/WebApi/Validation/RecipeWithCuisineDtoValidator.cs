using FluentValidation;
using JonathanPotts.RecipeCatalog.WebApi.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class RecipeWithCuisineDtoValidator : AbstractValidator<RecipeWithCuisineDto>
{
    public RecipeWithCuisineDtoValidator()
    {
        RuleFor(x => x.Cuisine).NotEmpty();
    }
}
