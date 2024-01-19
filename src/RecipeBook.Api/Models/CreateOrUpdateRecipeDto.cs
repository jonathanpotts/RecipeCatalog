using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class CreateOrUpdateRecipeDto
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
