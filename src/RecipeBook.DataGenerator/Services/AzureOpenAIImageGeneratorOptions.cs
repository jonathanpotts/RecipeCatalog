using System.ComponentModel.DataAnnotations;

namespace RecipeBook.DataGenerator.Services;

public class AzureOpenAIImageGeneratorOptions
{
    [Required]
    public string? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    [Required]
    public string? DeploymentName { get; set; }
}
