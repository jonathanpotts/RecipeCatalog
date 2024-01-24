using System.ComponentModel.DataAnnotations;

namespace RecipeBook.DataGenerator.Services;

public class ImageGeneratorOptions
{
    [Required]
    public string? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    public int? MaxRetries { get; set; }

    public int? RetryDelay { get; set; }
}
