using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeBook.AIDataGenerator.Services.Models;

public class AzureOpenAIImageGenerationRequest
{
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("n")]
    public int Count { get; set; } = 1;

    [JsonPropertyName("size")]
    public string? Size { get; set; } = "1024x1024";
}
