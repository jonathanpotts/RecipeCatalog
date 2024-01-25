using System.ComponentModel.DataAnnotations;

namespace JonathanPotts.RecipeBook.AIDataGenerator.Services;

public class AzureOpenAIDallE2ImageGeneratorOptions
{
    [Required]
    public string? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    public int? MaxRetries { get; set; }

    public int? RetryDelay { get; set; }
}
