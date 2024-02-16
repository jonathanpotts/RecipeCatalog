using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class CreateUpdateCuisineDtoValidator : AbstractValidator<CreateUpdateCuisineDto>
{
    public CreateUpdateCuisineDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
