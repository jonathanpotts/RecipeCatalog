using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class CuisineCreateOrUpdateDtoValidator : AbstractValidator<CuisineCreateOrUpdateDto>
{
    public CuisineCreateOrUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
