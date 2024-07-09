namespace RecipeCatalog.Application.Contracts.Models;

public class RecipeWithCuisineDto : RecipeDto
{
    public CuisineDto? Cuisine { get; set; }
}
