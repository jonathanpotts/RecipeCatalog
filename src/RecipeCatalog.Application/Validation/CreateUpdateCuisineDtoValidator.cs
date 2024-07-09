using FluentValidation;
using RecipeCatalog.Application.Contracts.Models;

namespace RecipeCatalog.Application.Validation;

public class CreateUpdateCuisineDtoValidator : AbstractValidator<CreateUpdateCuisineDto>
{
    public CreateUpdateCuisineDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
