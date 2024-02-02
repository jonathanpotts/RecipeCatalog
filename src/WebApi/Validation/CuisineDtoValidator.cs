using FluentValidation;
using JonathanPotts.RecipeCatalog.WebApi.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class CuisineDtoValidator : AbstractValidator<CuisineDto>
{
    public CuisineDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
