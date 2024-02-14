using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;

namespace JonathanPotts.RecipeCatalog.Domain.Entities;

public class Recipe
{
    public long Id { get; set; }

    public string? OwnerId { get; set; }

    public User? Owner { get; set; }

    public string? Name { get; set; }

    public ImageData? CoverImage { get; set; }

    public int CuisineId { get; set; }

    public Cuisine? Cuisine { get; set; }

    public string? Description { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    public string[]? Ingredients { get; set; }

    public MarkdownData? Instructions { get; set; }
}
