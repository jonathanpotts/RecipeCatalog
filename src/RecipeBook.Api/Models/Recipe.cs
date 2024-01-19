using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RecipeBook.Api.Models;

public class Recipe
{
    public long Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [JsonIgnore]
    public int CuisineId { get; set; }

    [Required]
    public Cuisine? Cuisine { get; set; }

    public string? Description { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [Required]
    public string[]? Ingredients { get; set; }

    [Required]
    public MarkdownData? Instructions { get; set; }
}
