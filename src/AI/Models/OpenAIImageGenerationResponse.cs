using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.AI.Models;

public class OpenAIImageGenerationResponse
{
    [JsonPropertyName("created")]
    public long? Created { get; set; }

    [JsonPropertyName("data")]
    public List<OpenAIImageGenerationResponseData>? Data { get; set; }
}
