using FluentValidation;
using JonathanPotts.RecipeCatalog.WebApi.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class CuisineCreateOrUpdateDtoValidator : AbstractValidator<CuisineCreateOrUpdateDto>
{
    public CuisineCreateOrUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
