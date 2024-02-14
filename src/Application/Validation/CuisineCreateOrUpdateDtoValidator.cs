using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.Application.Validation;

public class CuisineCreateOrUpdateDtoValidator : AbstractValidator<CuisineCreateOrUpdateDto>
{
    public CuisineCreateOrUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
