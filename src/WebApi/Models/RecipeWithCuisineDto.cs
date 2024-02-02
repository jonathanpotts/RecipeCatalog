namespace JonathanPotts.RecipeCatalog.WebApi.Models;

public class RecipeWithCuisineDto : RecipeDto
{
    public CuisineDto? Cuisine { get; set; }
}
