using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.WebApi.Models;

public class RecipeWithCuisineDto : RecipeDto
{
    [Required]
    public CuisineDto? Cuisine { get; set; }
}
