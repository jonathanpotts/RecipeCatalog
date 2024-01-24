using System.Text.Json.Serialization;

namespace RecipeBook.DataGenerator.Services.Models;

public class AzureOpenAIImageOperationResultData
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
