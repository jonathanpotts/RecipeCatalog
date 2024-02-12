using FluentValidation;
using JonathanPotts.RecipeCatalog.Shared.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class CuisineCreateOrUpdateDtoValidator : AbstractValidator<CuisineCreateOrUpdateDto>
{
    public CuisineCreateOrUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
