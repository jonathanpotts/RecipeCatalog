using FluentValidation;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class RecipeCreateOrUpdateDtoValidator : AbstractValidator<RecipeCreateOrUpdateDto>
{
    public RecipeCreateOrUpdateDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.CuisineId).NotEmpty();
        RuleFor(x => x.Ingredients).NotEmpty();
        RuleFor(x => x.Instructions).NotEmpty();
    }
}
