using System.Text.Json.Serialization;

namespace RecipeBook.DataGenerator.Services.Models;

public class AzureOpenAIImageOperationResponse
{
    [JsonPropertyName("created")]
    public long? Created { get; set; }

    [JsonPropertyName("expires")]
    public long? Expires { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("result")]
    public OpenAIImageGenerationResponse? Result { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
