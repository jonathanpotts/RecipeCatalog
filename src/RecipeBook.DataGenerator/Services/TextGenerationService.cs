using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace RecipeBook.DataGenerator.Services;

public sealed class TextGenerationService(IOptions<TextGenerationServiceOptions> options)
{
    private readonly string _chatCompletionsDeploymentName = options.Value.ChatCompletionsDeploymentName!;

    private readonly OpenAIClient _client = new(new Uri(options.Value.Endpoint!),
        new AzureKeyCredential(options.Value.ApiKey!));

    private readonly string _embeddingsDeploymentName = options.Value.EmbeddingsDeploymentName!;

    public async Task<T?> GenerateDataFromChatCompletions<T>(T exampleData, string systemMessage, string prompt,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(exampleData);

        ChatCompletionsOptions options = new(
            _chatCompletionsDeploymentName,
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

        var completions = await _client.GetChatCompletionsAsync(options, cancellationToken);

        return JsonSerializer.Deserialize<T>(completions.Value.Choices[0].Message.Content);
    }
}
