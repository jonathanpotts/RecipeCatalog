namespace JonathanPotts.RecipeCatalog.Shared.Models;

public class RecipeWithCuisineDto : RecipeDto
{
    public CuisineDto? Cuisine { get; set; }
}
