using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public class OpenAIImageGeneratorOptions
{
    [Required]
    public string? ApiKey { get; set; }

    public string? DeploymentName { get; set; }
}
