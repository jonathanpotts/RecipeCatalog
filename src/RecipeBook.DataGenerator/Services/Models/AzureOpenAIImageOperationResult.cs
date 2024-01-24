using System.Text.Json.Serialization;

namespace RecipeBook.DataGenerator.Services.Models;

public class AzureOpenAIImageOperationResult
{
    [JsonPropertyName("data")]
    public List<AzureOpenAIImageOperationResultData>? Data { get; set; }
}
