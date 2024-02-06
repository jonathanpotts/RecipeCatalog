namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public interface IAITextGenerator
{
    public Task<T?> GenerateDataFromChatCompletionsAsync<T>(T exampleData, string systemMessage, string prompt,
        CancellationToken cancellationToken = default);

    public Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string input,
        CancellationToken cancellationToken = default);
}
