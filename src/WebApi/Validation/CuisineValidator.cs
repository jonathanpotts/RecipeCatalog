using FluentValidation;
using JonathanPotts.RecipeCatalog.WebApi.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class CuisineValidator : AbstractValidator<Cuisine>
{
    public CuisineValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
