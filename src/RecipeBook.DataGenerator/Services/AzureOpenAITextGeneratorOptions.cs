using System.ComponentModel.DataAnnotations;

namespace RecipeBook.DataGenerator.Services;

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
