using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AI;

public class OpenAIImageGeneratorOptions
{
    [Required]
    public string? ApiKey { get; set; }

    public string? Size { get; set; }

    public string? Quality { get; set; }

    public string? Style { get; set; }
}
