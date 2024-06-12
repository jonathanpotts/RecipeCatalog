using System.Reflection;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.AI.Tests;

public sealed class OpenAITextGeneratorUnitTests
{
    [Fact]
    public async Task GenerateDataFromChatCompletionsAsyncReturnsData()
    {
        // Arrange
        var options = new OptionsWrapper<OpenAITextGeneratorOptions>(new()
        {
            ApiKey = "test-api-key"
        });

        var textGenerator = new OpenAITextGenerator(options);

        var chatResponseMessage = AzureOpenAIModelFactory.ChatResponseMessage(ChatRole.Assistant, "\"Test response\"");
        var chatChoice = AzureOpenAIModelFactory.ChatChoice(chatResponseMessage);
        var chatCompletions = AzureOpenAIModelFactory.ChatCompletions(choices: [chatChoice]);

        Mock<OpenAIClient> openAIClientMock = new();
        openAIClientMock
            .Setup(x => x.GetChatCompletionsAsync(It.IsAny<ChatCompletionsOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(Response.FromValue(chatCompletions, Mock.Of<Response>()));

        typeof(OpenAITextGenerator)
            .GetRuntimeFields()
            .FirstOrDefault(x => x.Name == "<ChatCompletionsClient>k__BackingField")!
            .SetValue(textGenerator, openAIClientMock.Object);

        // Act
        var result = await textGenerator.GenerateDataFromChatCompletionsAsync("test object", "test message", "test prompt");

        // Assert
        Assert.Equal("Test response", result);
    }

    [Fact]
    public async Task GenerateEmbeddingsAsyncReturnsFloatArray()
    {
        // Arrange
        var options = new OptionsWrapper<OpenAITextGeneratorOptions>(new()
        {
            ApiKey = "test-api-key"
        });

        var textGenerator = new OpenAITextGenerator(options);

        var embeddingItem = AzureOpenAIModelFactory.EmbeddingItem(ReadOnlyMemory<float>.Empty);
        var embeddings = AzureOpenAIModelFactory.Embeddings([embeddingItem]);

        Mock<OpenAIClient> openAIClientMock = new();
        openAIClientMock
            .Setup(x => x.GetEmbeddingsAsync(It.IsAny<EmbeddingsOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(Response.FromValue(embeddings, Mock.Of<Response>()));

        typeof(OpenAITextGenerator)
            .GetRuntimeFields()
            .FirstOrDefault(x => x.Name == "<ChatCompletionsClient>k__BackingField")!
            .SetValue(textGenerator, openAIClientMock.Object);

        // Act
        var result = await textGenerator.GenerateEmbeddingsAsync("test input");

        // Assert
        Assert.Equal(ReadOnlyMemory<float>.Empty, result);
    }
}
