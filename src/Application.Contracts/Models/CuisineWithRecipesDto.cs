namespace JonathanPotts.RecipeCatalog.Application.Contracts.Models;

public class CuisineWithRecipesDto : CuisineDto
{
    public List<RecipeDto>? Recipes { get; set; }
}
