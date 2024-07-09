using FluentValidation;
using RecipeCatalog.Application.Contracts.Models;

namespace RecipeCatalog.Application.Validation;

public class CuisineDtoValidator : AbstractValidator<CuisineDto>
{
    public CuisineDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
    }
}
