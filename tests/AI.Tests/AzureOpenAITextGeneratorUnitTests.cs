using System.Reflection;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.AI.Tests;

public sealed class AzureOpenAITextGeneratorUnitTests
{
    [Fact]
    public async void GenerateDataFromChatCompletionsAsyncCompletesSuccessfully()
    {
        // Arrange
        var options = new OptionsWrapper<AzureOpenAITextGeneratorOptions>(new()
        {
            Endpoint = "https://test-endpoint.openai.zure.com/",
            ApiKey = "test-api-key",
            ChatCompletionsDeploymentName = "gpt-35-turbo"
        });

        var textGenerator = new AzureOpenAITextGenerator(options);

        var chatResponseMessage = AzureOpenAIModelFactory.ChatResponseMessage(ChatRole.Assistant, "\"Test response\"");
        var chatChoice = AzureOpenAIModelFactory.ChatChoice(chatResponseMessage);
        var chatCompletions = AzureOpenAIModelFactory.ChatCompletions(choices: [chatChoice]);

        Mock<OpenAIClient> openAIClientMock = new();
        openAIClientMock
            .Setup(x => x.GetChatCompletionsAsync(It.IsAny<ChatCompletionsOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(Response.FromValue(chatCompletions, Mock.Of<Response>()));

        typeof(AzureOpenAITextGenerator)
            .GetRuntimeFields()
            .FirstOrDefault(x => x.Name == "<ChatCompletionsClient>k__BackingField")!
            .SetValue(textGenerator, openAIClientMock.Object);

        // Act
        var result = await textGenerator.GenerateDataFromChatCompletionsAsync("test object", "test message", "test prompt");

        // Assert
        Assert.Equal("Test response", result);
    }

    [Fact]
    public async void GenerateDataFromChatCompletionsAsyncThrowsWhenDeploymentNameMissing()
    {
        // Arrange
        var options = new OptionsWrapper<AzureOpenAITextGeneratorOptions>(new()
        {
            Endpoint = "https://test-endpoint.openai.zure.com/",
            ApiKey = "test-api-key"
        });

        var textGenerator = new AzureOpenAITextGenerator(options);

        // Act / Assert
        await Assert.ThrowsAsync<Exception>(() => textGenerator.GenerateDataFromChatCompletionsAsync("test object", "test message", "test prompt"));
    }

    [Fact]
    public async void GenerateEmbeddingsAsyncCompletesSuccessfully()
    {
        // Arrange
        var options = new OptionsWrapper<AzureOpenAITextGeneratorOptions>(new()
        {
            Endpoint = "https://test-endpoint.openai.zure.com/",
            ApiKey = "test-api-key",
            EmbeddingsDeploymentName = "text-embedding-3-small"
        });

        var textGenerator = new AzureOpenAITextGenerator(options);

        var embeddingItem = AzureOpenAIModelFactory.EmbeddingItem(ReadOnlyMemory<float>.Empty);
        var embeddings = AzureOpenAIModelFactory.Embeddings([embeddingItem]);

        Mock<OpenAIClient> openAIClientMock = new();
        openAIClientMock
            .Setup(x => x.GetEmbeddingsAsync(It.IsAny<EmbeddingsOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(Response.FromValue(embeddings, Mock.Of<Response>()));

        typeof(AzureOpenAITextGenerator)
            .GetRuntimeFields()
            .FirstOrDefault(x => x.Name == "<EmbeddingsClient>k__BackingField")!
            .SetValue(textGenerator, openAIClientMock.Object);

        // Act
        var result = await textGenerator.GenerateEmbeddingsAsync("test input");

        // Assert
        Assert.Equal(ReadOnlyMemory<float>.Empty, result);
    }

    [Fact]
    public async void GenerateEmbeddingsAsyncThrowsWhenDeploymentNameMissing()
    {
        // Arrange
        var options = new OptionsWrapper<AzureOpenAITextGeneratorOptions>(new()
        {
            Endpoint = "https://test-endpoint.openai.zure.com/",
            ApiKey = "test-api-key"
        });

        var textGenerator = new AzureOpenAITextGenerator(options);

        // Act / Assert
        await Assert.ThrowsAsync<Exception>(() => textGenerator.GenerateEmbeddingsAsync("test input"));
    }
}
