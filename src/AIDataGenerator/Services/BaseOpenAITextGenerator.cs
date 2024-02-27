using System.Text.Json;
using Azure.AI.OpenAI;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public abstract class BaseOpenAITextGenerator : IAITextGenerator
{
    protected abstract OpenAIClient ChatCompletionsClient { get; }

    protected abstract OpenAIClient EmbeddingsClient { get; }

    public abstract Task<T?> GenerateDataFromChatCompletionsAsync<T>(T exampleData, string systemMessage, string prompt,
        CancellationToken cancellationToken = default);

    public abstract Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string input,
        CancellationToken cancellationToken = default);

    protected async Task<T?> GenerateDataFromChatCompletionsAsync<T>(string deploymentName, T exampleData,
        string systemMessage, string prompt, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(exampleData);

        ChatCompletionsOptions options = new(
            deploymentName,
            [
                new ChatRequestSystemMessage($"""
                                              {systemMessage}

                                              Respond using the following JSON format:
                                              {json}
                                              """),
                new ChatRequestUserMessage(prompt)
            ])
        {
            ResponseFormat = ChatCompletionsResponseFormat.JsonObject
        };

        var completions = await ChatCompletionsClient.GetChatCompletionsAsync(options, cancellationToken);

        return JsonSerializer.Deserialize<T>(completions.Value.Choices[0].Message.Content);
    }

    protected async Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string deploymentName, string input,
        CancellationToken cancellationToken = default)
    {
        var embeddings =
            await EmbeddingsClient.GetEmbeddingsAsync(new EmbeddingsOptions(deploymentName, [input]), cancellationToken);

        return embeddings.Value.Data[0].Embedding;
    }
}
