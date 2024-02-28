using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.AI.Models;

public class OpenAIImageGenerationResponseData
{
    [JsonPropertyName("b64_json")]
    public string? Base64Json { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("revised_prompt")]
    public string? RevisedPrompt { get; set; }
}
