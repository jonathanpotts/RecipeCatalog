using FluentValidation;
using RecipeCatalog.Application.Contracts.Models;

namespace RecipeCatalog.Application.Validation;

public class RecipeWithCuisineDtoValidator : AbstractValidator<RecipeWithCuisineDto>
{
    public RecipeWithCuisineDtoValidator()
    {
        Include(new RecipeDtoValidator());

        RuleFor(x => x.Cuisine!).NotNull()
            .SetValidator(new CuisineDtoValidator());
    }
}
