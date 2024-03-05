using IdGen;
using JonathanPotts.RecipeCatalog.AI;
using JonathanPotts.RecipeCatalog.Application.Contracts.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JonathanPotts.RecipeCatalog.Application.Tests;

public sealed class ExtensionsUnitTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async void AddApplicationServicesReturnsPopulatedServiceProvider(bool hasGeneratorId, bool hasOpenAIApiKey)
    {
        // Arrange
        ServiceCollection services = new();

        Dictionary<string, string?> configurationValues = [];

        if (hasGeneratorId)
        {
            configurationValues.Add("GeneratorId", "0");
        }

        if (hasOpenAIApiKey)
        {
            configurationValues.Add("OpenAI:ApiKey", "test-key");
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

        if (hasOpenAIApiKey)
        {
            Assert.NotNull(serviceProvider.GetService<IAITextGenerator>());
        }
    }
}
