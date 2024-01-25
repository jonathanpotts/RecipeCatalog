using System.ComponentModel.DataAnnotations;

namespace RecipeBook.DataGenerator.Services;

public class OpenAIImageGeneratorOptions
{
    [Required]
    public string? ApiKey { get; set; }

    public string? DeploymentName { get; set; }
}
