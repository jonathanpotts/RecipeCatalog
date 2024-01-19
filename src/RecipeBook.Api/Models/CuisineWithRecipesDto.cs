using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class CuisineWithRecipesDto : CuisineDto
{
    [Required]
    public List<Recipe>? Recipes { get; set; }
}
