using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.Shared.Models;

public class RecipeDto
{
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public long Id { get; set; }

    public string? OwnerId { get; set; }

    public string? Name { get; set; }

    public ImageData? CoverImage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Modified { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Ingredients { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull)]
    public MarkdownData? Instructions { get; set; }
}
