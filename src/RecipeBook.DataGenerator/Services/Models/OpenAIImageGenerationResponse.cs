using System.Text.Json.Serialization;

namespace RecipeBook.DataGenerator.Services.Models;

public class OpenAIImageGenerationResponse
{
    [JsonPropertyName("created")]
    public long? Created { get; set; }

    [JsonPropertyName("data")]
    public List<OpenAIImageGenerationResponseData>? Data { get; set; }
}
