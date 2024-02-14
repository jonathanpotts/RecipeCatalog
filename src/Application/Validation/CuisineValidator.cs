using FluentValidation;
using JonathanPotts.RecipeCatalog.Domain.Entities;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class CuisineValidator : AbstractValidator<Cuisine>
{
    public CuisineValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
