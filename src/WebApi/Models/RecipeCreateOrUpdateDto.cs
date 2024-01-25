using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.WebApi.Models;

public class RecipeCreateOrUpdateDto
{
    [Required]
    public string? Name { get; set; }

    public int CuisineId { get; set; }

    public string? Description { get; set; }

    [Required]
    public string[]? Ingredients { get; set; }

    [Required]
    public string? Instructions { get; set; }
}
