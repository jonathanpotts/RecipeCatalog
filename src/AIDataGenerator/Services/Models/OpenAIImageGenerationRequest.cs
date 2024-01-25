using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services.Models;

public class OpenAIImageGenerationRequest : AzureOpenAIImageGenerationRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; } = "dall-e-2";

    [JsonPropertyName("response_format")]
    public string? ResponseFormat { get; set; } = "url";
}
