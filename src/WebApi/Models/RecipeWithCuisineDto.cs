using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeBook.WebApi.Models;

public class RecipeWithCuisineDto : RecipeDto
{
    [Required]
    public CuisineDto? Cuisine { get; set; }
}
