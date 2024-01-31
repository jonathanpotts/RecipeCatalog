using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.WebApi.Models;

public class RecipeDto
{
    [JsonNumberHandling(JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString)]
    public long Id { get; set; }

    [Required]
    public string? OwnerId { get; set; }

    [Required]
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
