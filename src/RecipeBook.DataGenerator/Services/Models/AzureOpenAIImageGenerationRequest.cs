using System.Text.Json.Serialization;

namespace RecipeBook.DataGenerator.Services.Models;

public class AzureOpenAIImageGenerationRequest
{
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("size")]
    public string? Size { get; set; } = "1024x1024";

    [JsonPropertyName("n")]
    public int Count { get; set; } = 1;
}
