using System.Reflection;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.AI.Tests;

public sealed class OpenAIImageGeneratorUnitTests
{
    [Theory]
    [InlineData(null, null, null)]
    [InlineData("1024x1024", null, null)]
    [InlineData("1024x1792", null, null)]
    [InlineData("1792x1024", null, null)]
    [InlineData(null, "standard", null)]
    [InlineData(null, "hd", null)]
    [InlineData(null, null, "vivid")]
    [InlineData(null, null, "natural")]
    public async void GenerateImageAsyncReturnsString(string? size, string? quality, string? style)
    {
        // Arrange
        var options = new OptionsWrapper<OpenAIImageGeneratorOptions>(new()
        {
            ApiKey = "test-api-key",
            Size = size,
            Quality = quality,
            Style = style
        });

        var imageGenerator = new OpenAIImageGenerator(options);

        var imageGenerationData = AzureOpenAIModelFactory.ImageGenerationData(new Uri("https://test/image.png"));
        var imageGenerations = new ImageGenerations(DateTimeOffset.UtcNow, [imageGenerationData]);

        Mock<OpenAIClient> openAIClientMock = new();
        openAIClientMock
            .Setup(x => x.GetImageGenerationsAsync(It.IsAny<ImageGenerationOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(Response.FromValue(imageGenerations, Mock.Of<Response>()));

        typeof(OpenAIImageGenerator)
            .GetRuntimeFields()
            .FirstOrDefault(x => x.Name == "<Client>k__BackingField")!
            .SetValue(imageGenerator, openAIClientMock.Object);

        // Act
        var result = await imageGenerator.GenerateImageAsync("test prompt");

        // Assert
        Assert.Equal("https://test/image.png", result);
    }
}
