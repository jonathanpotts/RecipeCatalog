using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeBook.WebApi.Models;

public class CuisineCreateOrUpdateDto
{
    [Required]
    public string? Name { get; set; }
}
