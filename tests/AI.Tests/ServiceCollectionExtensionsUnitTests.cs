using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.AI.Tests;

public sealed class ServiceCollectionExtensionsUnitTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AddAITextGeneratorReturnsPopulatedServiceCollection(bool azure)
    {
        // Arrange
        Dictionary<string, string?> configurationValues = [];
        configurationValues.Add("ApiKey", "test-api-key");

        if (azure)
        {
            configurationValues.Add("Endpoint", "https://test-endpoint.openai.azure.com/");
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        ServiceCollection services = new();

        // Act
        services.AddAITextGenerator(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IAITextGenerator>());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AddAIImageGeneratorReturnsPopulatedServiceCollection(bool azure)
    {
        // Arrange
        Dictionary<string, string?> configurationValues = [];
        configurationValues.Add("ApiKey", "test-api-key");

        if (azure)
        {
            configurationValues.Add("Endpoint", "https://test-endpoint.openai.azure.com/");
            configurationValues.Add("DeploymentName", "dall-e-3");
        }

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        ServiceCollection services = new();

        // Act
        services.AddAIImageGenerator(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IAIImageGenerator>());
    }

    [Fact]
    public void AddOpenAITextGeneratorActionReturnsPopulatedServiceCollection()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddOpenAITextGenerator(options =>
        {
            options.ApiKey = "test-api-key";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IAITextGenerator>());
    }

    [Fact]
    public void AddAzureOpenAITextGeneratorActionReturnsPopulatedServiceCollection()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddAzureOpenAITextGenerator(options =>
        {
            options.Endpoint = "https://test-endpoint.openai.azure.com/";
            options.ApiKey = "test-api-key";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IAITextGenerator>());
    }

    [Fact]
    public void AddOpenAIImageGeneratorActionReturnsPopulatedServiceCollection()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddOpenAIImageGenerator(options =>
        {
            options.ApiKey = "test-api-key";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IAIImageGenerator>());
    }

    [Fact]
    public void AddAzureOpenAIImageGeneratorActionReturnsPopulatedServiceCollection()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddAzureOpenAIImageGenerator(options =>
        {
            options.Endpoint = "https://test-endpoint.openai.azure.com/";
            options.ApiKey = "test-api-key";
            options.DeploymentName = "dall-e-3";
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IAIImageGenerator>());
    }
}
