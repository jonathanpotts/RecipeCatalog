using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public class OpenAITextGeneratorOptions
{
    [Required]
    public string? ApiKey { get; set; }

    public string? ChatCompletionsDeploymentName { get; set; }

    public string? EmbeddingsDeploymentName { get; set; }
}
