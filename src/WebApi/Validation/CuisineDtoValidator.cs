using FluentValidation;
using JonathanPotts.RecipeCatalog.Shared.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class CuisineDtoValidator : AbstractValidator<CuisineDto>
{
    public CuisineDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
