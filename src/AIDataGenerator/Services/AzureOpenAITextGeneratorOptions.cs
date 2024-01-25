using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public class AzureOpenAITextGeneratorOptions
{
    [Required]
    public string? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    [Required]
    public string? ChatCompletionsDeploymentName { get; set; }

    [Required]
    public string? EmbeddingsDeploymentName { get; set; }
}
