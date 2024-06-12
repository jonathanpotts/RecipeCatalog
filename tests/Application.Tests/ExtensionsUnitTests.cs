using IdGen;
using JonathanPotts.RecipeCatalog.Application.Contracts.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Embeddings;

namespace JonathanPotts.RecipeCatalog.Application.Tests;

public sealed class ExtensionsUnitTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task AddApplicationServicesReturnsPopulatedServiceProvider(bool hasGeneratorId, bool hasTextEmbeddingGenerationService)
    {
        // Arrange
        ServiceCollection services = new();

        Dictionary<string, string?> configurationValues = [];

        if (hasGeneratorId)
        {
            configurationValues.Add("GeneratorId", "0");
        }

        if (hasTextEmbeddingGenerationService)
        {
            configurationValues.Add("OpenAI:TextEmbedding:Model", "text-embedding-3-small");
            configurationValues.Add("OpenAI:TextEmbedding:Key", "test-key");
        }

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        // Act
        services.AddApplicationServices(configuration);
        services.AddIdentityBlazor();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<RecipeCatalogDbContext>());
        Assert.NotNull(serviceProvider.GetService<IIdGenerator<long>>());
        Assert.NotNull(serviceProvider.GetService<ICuisineService>());
        Assert.NotNull(serviceProvider.GetService<IRecipeService>());
        Assert.NotNull(serviceProvider.GetService<IAuthorizationService>());
        Assert.NotNull(serviceProvider.GetService<IAuthorizationPolicyProvider>());
        Assert.NotNull(await serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>().GetPolicyAsync(Policies.Create));
        Assert.NotNull(await serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>().GetPolicyAsync(Policies.Read));
        Assert.NotNull(await serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>().GetPolicyAsync(Policies.Update));
        Assert.NotNull(await serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>().GetPolicyAsync(Policies.Delete));
        Assert.NotNull(serviceProvider.GetService<UserManager<User>>());
        Assert.NotNull(serviceProvider.GetService<SignInManager<User>>());

        if (hasTextEmbeddingGenerationService)
        {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            Assert.NotNull(serviceProvider.GetService<ITextEmbeddingGenerationService>());
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }
    }
}
