namespace RecipeBook.DataGenerator.Services;

public interface ITextGenerator
{
    public Task<T?> GenerateDataFromChatCompletions<T>(T exampleData, string systemMessage, string prompt,
        CancellationToken cancellationToken = default);

    public Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string input, CancellationToken cancellationToken = default);
}
