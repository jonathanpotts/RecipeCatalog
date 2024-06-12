using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Options;

public class TextEmbedding
{
    [Required]
    public required string Model { get; set; }

    [Required]
    public required string Key { get; set; }

    public string? Endpoint { get; set; }
}
