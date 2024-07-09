namespace RecipeCatalog.Application.Contracts.Models;

public class CreateUpdateRecipeDto
{
    public string? Name { get; set; }

    public int CuisineId { get; set; }

    public string? Description { get; set; }

    public string[]? Ingredients { get; set; }

    public string? Instructions { get; set; }
}
