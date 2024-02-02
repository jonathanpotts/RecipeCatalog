using FluentValidation;
using JonathanPotts.RecipeCatalog.WebApi.Models;

namespace JonathanPotts.RecipeCatalog.WebApi.Validation;

public class RecipeValidator : AbstractValidator<Recipe>
{
    public RecipeValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.CuisineId).NotEmpty();
        RuleFor(x => x.Ingredients).NotEmpty();
        RuleFor(x => x.Ingredients).NotEmpty();
    }
}
