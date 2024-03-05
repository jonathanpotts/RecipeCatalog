using System.Security.Claims;
using IdGen;
using JonathanPotts.RecipeCatalog.AI;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.Application.Tests;

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

    public static Mock<IServiceProvider> CreateServiceProviderMock(bool withAITextGenerator = false)
    {
        Mock<IServiceProvider> serviceProviderMock = new();

        if (withAITextGenerator)
        {
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IAITextGenerator)))
                .Returns(CreateAITextGeneratorMock().Object);
        }

        return serviceProviderMock;
    }

    public static Mock<IAITextGenerator> CreateAITextGeneratorMock()
    {
        Mock<IAITextGenerator> aiTextGeneratorMock = new();

        aiTextGeneratorMock
            .Setup(x => x.GenerateEmbeddingsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new ReadOnlyMemory<float>(new float[1536]));

        return aiTextGeneratorMock;
    }
}
