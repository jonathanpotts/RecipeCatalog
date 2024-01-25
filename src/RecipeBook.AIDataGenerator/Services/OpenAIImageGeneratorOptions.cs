using System.ComponentModel.DataAnnotations;

namespace RecipeBook.AIDataGenerator.Services;

public class OpenAIImageGeneratorOptions
{
    [Required]
    public string? ApiKey { get; set; }

    public string? DeploymentName { get; set; }
}
