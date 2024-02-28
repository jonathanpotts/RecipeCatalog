using System.Text.Json.Serialization;

namespace JonathanPotts.RecipeCatalog.AI.Models;

public class AzureOpenAIImageGenerationResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
