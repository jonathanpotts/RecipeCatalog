using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace RecipeBook.DataGenerator.Services;

public class OpenAITextGenerator(IOptions<OpenAITextGeneratorOptions> options) : BaseOpenAITextGenerator
{
    private readonly string _chatCompletionsDeploymentName =
        options.Value.ChatCompletionsDeploymentName ?? "gpt-3.5-turbo-1106";

    private readonly string _embeddingsDeploymentName =
        options.Value.EmbeddingsDeploymentName ?? "text-embedding-ada-002";

    protected override OpenAIClient Client { get; init; } = new(options.Value.ApiKey);

    public override Task<T?> GenerateDataFromChatCompletions<T>(T exampleData, string systemMessage, string prompt,
        CancellationToken cancellationToken = default) where T : default
    {
        return GenerateDataFromChatCompletions(_chatCompletionsDeploymentName, exampleData, systemMessage, prompt,
            cancellationToken);
    }

    public override Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string input, CancellationToken cancellationToken = default)
    {
        return GenerateEmbeddingsAsync(_embeddingsDeploymentName, input, cancellationToken);
    }
}
