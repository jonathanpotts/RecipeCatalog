using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services.Models;

public class AzureOpenAIImageGenerationResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
