using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace RecipeBook.DataGenerator.Services;

public class AzureOpenAITextGenerator(IOptions<AzureOpenAITextGeneratorOptions> options) : BaseOpenAITextGenerator
{
    private readonly string _chatCompletionsDeploymentName = options.Value.ChatCompletionsDeploymentName!;
    private readonly string _embeddingsDeploymentName = options.Value.EmbeddingsDeploymentName!;

    protected override OpenAIClient Client { get; init; } = new(new Uri(options.Value.Endpoint!),
        new AzureKeyCredential(options.Value.ApiKey!));

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
