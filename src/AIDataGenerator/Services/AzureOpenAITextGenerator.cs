using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator.Services;

public class AzureOpenAITextGenerator(IOptions<AzureOpenAITextGeneratorOptions> options) : BaseOpenAITextGenerator
{
    private readonly string _chatCompletionsDeploymentName = options.Value.ChatCompletionsDeploymentName!;
    private readonly string _embeddingsDeploymentName = options.Value.EmbeddingsDeploymentName!;

    protected override OpenAIClient ChatCompletionsClient { get; } = new(new Uri(options.Value.ChatCompletionsEndpoint!),
        new AzureKeyCredential(options.Value.ChatCompletionsApiKey!));

    protected override OpenAIClient EmbeddingsClient { get; } = new(new Uri(options.Value.EmbeddingsEndpoint ?? options.Value.ChatCompletionsEndpoint!),
        new AzureKeyCredential(options.Value.EmbeddingsApiKey ?? options.Value.ChatCompletionsApiKey!));

    public override Task<T?> GenerateDataFromChatCompletionsAsync<T>(T exampleData, string systemMessage, string prompt,
        CancellationToken cancellationToken = default) where T : default
    {
        return GenerateDataFromChatCompletionsAsync(_chatCompletionsDeploymentName, exampleData, systemMessage, prompt,
            cancellationToken);
    }

    public override Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string input,
        CancellationToken cancellationToken = default)
    {
        return GenerateEmbeddingsAsync(_embeddingsDeploymentName, input, cancellationToken);
    }
}
