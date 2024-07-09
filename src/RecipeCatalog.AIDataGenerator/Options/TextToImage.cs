using System.ComponentModel.DataAnnotations;

namespace RecipeCatalog.AIDataGenerator.Options;

public class TextToImage
{
    [Required]
    public required string Key { get; set; }

    public string? Endpoint { get; set; }

    public string? Deployment { get; set; }
}
