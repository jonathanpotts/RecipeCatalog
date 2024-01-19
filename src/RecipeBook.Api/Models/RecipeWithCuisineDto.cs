using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class RecipeWithCuisineDto : RecipeDto
{
    [Required]
    public CuisineDto? Cuisine { get; set; }
}
