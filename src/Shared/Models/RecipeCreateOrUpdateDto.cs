namespace JonathanPotts.RecipeCatalog.Shared.Models;

public class RecipeCreateOrUpdateDto
{
    public string? Name { get; set; }

    public int CuisineId { get; set; }

    public string? Description { get; set; }

    public string[]? Ingredients { get; set; }

    public string? Instructions { get; set; }
}
