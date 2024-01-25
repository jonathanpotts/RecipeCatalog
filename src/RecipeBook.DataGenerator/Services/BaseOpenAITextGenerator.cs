﻿using System.Text.Json;
using Azure.AI.OpenAI;

namespace RecipeBook.DataGenerator.Services;

public abstract class BaseOpenAITextGenerator : ITextGenerator
{
    protected abstract OpenAIClient Client { get; init; }

    public abstract Task<T?> GenerateDataFromChatCompletions<T>(T exampleData, string systemMessage, string prompt,
        CancellationToken cancellationToken = default);

    public abstract Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string input, CancellationToken cancellationToken = default);

    protected async Task<T?> GenerateDataFromChatCompletions<T>(string deploymentName, T exampleData,
        string systemMessage, string prompt, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(exampleData);

        ChatCompletionsOptions options = new(
            deploymentName,
            new List<ChatRequestMessage>
            {
                new ChatRequestSystemMessage($"""
                                              {systemMessage}

                                              Respond using the following JSON format:
                                              {json}
                                              """),
                new ChatRequestUserMessage(prompt)
            })
        {
            ResponseFormat = ChatCompletionsResponseFormat.JsonObject
        };

        var completions = await Client.GetChatCompletionsAsync(options, cancellationToken);

        return JsonSerializer.Deserialize<T>(completions.Value.Choices[0].Message.Content);
    }

    protected async Task<ReadOnlyMemory<float>> GenerateEmbeddingsAsync(string deploymentName, string input,
        CancellationToken cancellationToken = default)
    {
        var embeddings = await Client.GetEmbeddingsAsync(new EmbeddingsOptions(deploymentName, [input]), cancellationToken);

        return embeddings.Value.Data[0].Embedding;
    }
}
