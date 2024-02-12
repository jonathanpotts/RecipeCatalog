namespace JonathanPotts.RecipeCatalog.Shared.Models;

public class CuisineWithRecipesDto : CuisineDto
{
    public List<RecipeDto>? Recipes { get; set; }
}
