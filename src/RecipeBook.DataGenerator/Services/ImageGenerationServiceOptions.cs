using System.ComponentModel.DataAnnotations;

namespace RecipeBook.DataGenerator.Services;

public class ImageGenerationServiceOptions
{
    [Required]
    public string? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }

    public int? MaxRetries { get; set; }
}
