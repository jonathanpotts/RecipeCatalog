using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeCatalog.AI;

public class AzureOpenAITextGeneratorOptions
{
    [Required]
    public string? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    public string? ChatCompletionsDeploymentName { get; set; }

    public string? EmbeddingsEndpoint { get; set; }

    public string? EmbeddingsApiKey { get; set; }

    public string? EmbeddingsDeploymentName { get; set; }
}
