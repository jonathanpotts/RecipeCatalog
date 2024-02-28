using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AI;

public class AzureOpenAIImageGeneratorOptions
{
    [Required]
    public string? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    [Required]
    public string? DeploymentName { get; set; }

    public string? Size { get; set; }

    public string? Quality { get; set; }

    public string? Style { get; set; }
}
