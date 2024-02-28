using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.AI.Models;

public class OpenAIImageGenerationRequest : AzureOpenAIImageGenerationRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; } = "dall-e-2";

    [JsonPropertyName("response_format")]
    public string? ResponseFormat { get; set; } = "url";
}
