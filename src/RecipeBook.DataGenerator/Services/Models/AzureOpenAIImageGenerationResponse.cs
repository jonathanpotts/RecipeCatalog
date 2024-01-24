using System.Text.Json.Serialization;

namespace RecipeBook.DataGenerator.Services.Models;

public class AzureOpenAIImageGenerationResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
