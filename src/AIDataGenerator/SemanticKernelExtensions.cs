using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator;

internal static class SemanticKernelExtensions
{
    private static readonly OpenAIPromptExecutionSettings s_executionSettings = new()
    {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        ResponseFormat = ChatCompletionsResponseFormat.JsonObject,
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    };

    public static async Task<T?> GenerateDataFromChatCompletionsAsync<T>(this IChatCompletionService chatCompletionService, T exampleData,
        string systemMessage, string prompt, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(exampleData);

        ChatHistory chatHistory = new($"""
            {systemMessage}

            Respond using the following JSON format:
            {json}
            """);

        chatHistory.AddUserMessage(prompt);

        var content = await chatCompletionService.GetChatMessageContentAsync(chatHistory, s_executionSettings, cancellationToken: cancellationToken);

        return JsonSerializer.Deserialize<T>(content.ToString());
    }
}
