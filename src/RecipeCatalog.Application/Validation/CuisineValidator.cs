using FluentValidation;
using RecipeCatalog.Domain.Entities;

namespace RecipeCatalog.Application.Validation;

public class CuisineValidator : AbstractValidator<Cuisine>
{
    public CuisineValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
