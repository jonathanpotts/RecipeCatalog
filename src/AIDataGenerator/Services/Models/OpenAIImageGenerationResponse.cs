using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeBook.AIDataGenerator.Services.Models;

public class OpenAIImageGenerationResponse
{
    [JsonPropertyName("created")]
    public long? Created { get; set; }

    [JsonPropertyName("data")]
    public List<OpenAIImageGenerationResponseData>? Data { get; set; }
}
