namespace RecipeCatalog.Domain.Entities;

public class Cuisine
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public List<Recipe>? Recipes { get; set; }
}
