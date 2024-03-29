﻿using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace JonathanPotts.RecipeCatalog.AI;

public class OpenAITextGenerator(IOptions<OpenAITextGeneratorOptions> options) : BaseOpenAITextGenerator
{
    private readonly string _chatCompletionsDeploymentName =
        options.Value.ChatCompletionsDeploymentName ?? "gpt-3.5-turbo";

    private readonly string _embeddingsDeploymentName =
        options.Value.EmbeddingsDeploymentName ?? "text-embedding-3-small";

    protected override OpenAIClient ChatCompletionsClient { get; } = new(options.Value.ApiKey);

    protected override OpenAIClient EmbeddingsClient => ChatCompletionsClient;

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
