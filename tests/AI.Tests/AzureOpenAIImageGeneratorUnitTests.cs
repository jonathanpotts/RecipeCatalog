using System.Reflection;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.AI.Tests;

public sealed class AzureOpenAIImageGeneratorUnitTests
{
    [Fact]
    public async void GenerateImageAsyncReturnsString()
    {
        // Arrange
        var options = new OptionsWrapper<AzureOpenAIImageGeneratorOptions>(new()
        {
            Endpoint = "https://test-endpoint.openai.zure.com/",
            ApiKey = "test-api-key",
            DeploymentName = "dall-e-3"
        });

        var imageGenerator = new AzureOpenAIImageGenerator(options);

        var imageGenerationData = AzureOpenAIModelFactory.ImageGenerationData(new Uri("https://test/image.png"));
        var imageGenerations = new ImageGenerations(DateTimeOffset.UtcNow, [imageGenerationData]);

        Mock<OpenAIClient> openAIClientMock = new();
        openAIClientMock
            .Setup(x => x.GetImageGenerationsAsync(It.IsAny<ImageGenerationOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(Response.FromValue(imageGenerations, Mock.Of<Response>()));

        typeof(AzureOpenAIImageGenerator)
            .GetRuntimeFields()
            .FirstOrDefault(x => x.Name == "<Client>k__BackingField")!
            .SetValue(imageGenerator, openAIClientMock.Object);

        // Act
        var result = await imageGenerator.GenerateImageAsync("test prompt");

        // Assert
        Assert.Equal("https://test/image.png", result);
    }
}
