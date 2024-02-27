using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public class AzureOpenAITextGeneratorOptions
{
    [Required]
    public string? ChatCompletionsEndpoint { get; set; }

    [Required]
    public string? ChatCompletionsApiKey { get; set; }

    [Required]
    public string? ChatCompletionsDeploymentName { get; set; }

    [Required]
    public string? EmbeddingsEndpoint { get; set; }

    [Required]
    public string? EmbeddingsApiKey { get; set; }

    [Required]
    public string? EmbeddingsDeploymentName { get; set; }
}
