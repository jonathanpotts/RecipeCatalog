using System.ComponentModel.DataAnnotations;

namespace RecipeCatalog.AIDataGenerator.Options;

public class ChatCompletion
{
    [Required]
    public required string Model { get; set; }

    [Required]
    public required string Key { get; set; }

    public string? Endpoint { get; set; }
}
