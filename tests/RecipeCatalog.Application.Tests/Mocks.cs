﻿using System.Security.Claims;
using IdGen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Moq;
using RecipeCatalog.Domain.Entities;
using RecipeCatalog.Tests.Shared;

namespace RecipeCatalog.Application.Tests;

public static class Mocks
{
    public const long GeneratedId = 23430727760609280;

    public static Mock<UserManager<User>> CreateUserManagerMock()
    {
        Mock<UserManager<User>> userManagerMock = new(
            Mock.Of<IUserStore<User>>(),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<User>>(),
            Array.Empty<IUserValidator<User>>(),
            Array.Empty<IPasswordValidator<User>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<User>>>());

        userManagerMock
            .Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .CallBase();
        userManagerMock
            .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()).Result)
            .Returns((ClaimsPrincipal x) => new User { Id = x.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty });
        userManagerMock
            .Setup(x => x.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()).Result)
            .Returns(false);
        userManagerMock
            .Setup(x => x.IsInRoleAsync(It.Is<User>(x => TestData.IsAdministrator(x.Id)), It.Is<string>(x => TestData.IsAdministratorRole(x))).Result)
            .Returns(true);

        return userManagerMock;
    }

    public static Mock<IIdGenerator<long>> CreateIdGeneratorMock()
    {
        Mock<IIdGenerator<long>> idGeneratorMock = new();

        idGeneratorMock.Setup(x => x.CreateId()).Returns(GeneratedId);

        return idGeneratorMock;
    }

    public static Mock<IAuthorizationService> CreateAuthorizationServiceMock(bool succeeds = true)
    {
        Mock<IAuthorizationService> authorizationServiceMock = new();

        authorizationServiceMock
            .Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object?>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()).Result)
            .Returns(succeeds ? AuthorizationResult.Success() : AuthorizationResult.Failed());

        return authorizationServiceMock;
    }

    public static Mock<IServiceProvider> CreateServiceProviderMock(bool withTextEmbeddingGenerationService = false)
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        if (withTextEmbeddingGenerationService)
        {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            serviceProviderMock
                .Setup(x => x.GetService(typeof(ITextEmbeddingGenerationService)))
                .Returns(CreateTextEmbeddingGenerationServiceMock().Object);
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        return serviceProviderMock;
    }

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    public static Mock<ITextEmbeddingGenerationService> CreateTextEmbeddingGenerationServiceMock()
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        Mock<ITextEmbeddingGenerationService> textEmbeddingGenerationServiceMock = new();
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        textEmbeddingGenerationServiceMock
            .Setup(x => x.GenerateEmbeddingsAsync(It.IsAny<IList<string>>(), It.IsAny<Kernel>(), It.IsAny<CancellationToken>()).Result)
            .Returns([new ReadOnlyMemory<float>(TestData.GetExampleNameEmbeddings())]);

        return textEmbeddingGenerationServiceMock;
    }
}
