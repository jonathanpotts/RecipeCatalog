using System.ComponentModel.DataAnnotations;

namespace RecipeBook.Api.Models;

public class Cuisine
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public List<Recipe>? Recipes { get; set; }
}
