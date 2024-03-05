using System.Security.Claims;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Authorization;

public class RecipeAuthorizationHandlerUnitTests
{

    private readonly RecipeAuthorizationHandler _handler;

    private readonly Recipe _recipe;

    private readonly ClaimsPrincipal _admin = new(new ClaimsIdentity(
    [
        new (ClaimTypes.NameIdentifier, "73edf737-df51-4c06-ac6f-3ec6d79f1f12")
    ], "Test"));

    private readonly ClaimsPrincipal _owner = new(new ClaimsIdentity(
    [
        new (ClaimTypes.NameIdentifier, "d7df5331-1c53-491f-8b71-91989846874f")
    ], "Test"));

    public RecipeAuthorizationHandlerUnitTests()
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
            .Setup(x => x.IsInRoleAsync(It.Is<User>(x => x.Id == "73edf737-df51-4c06-ac6f-3ec6d79f1f12"), "Administrator").Result)
            .Returns(true);
        userManagerMock
            .Setup(x => x.IsInRoleAsync(It.Is<User>(x => x.Id != "73edf737-df51-4c06-ac6f-3ec6d79f1f12"), "Administrator").Result)
            .Returns(false);

        _handler = new(userManagerMock.Object);

        _recipe = new()
        {
            Id = 6461870173061120,
            OwnerId = "d7df5331-1c53-491f-8b71-91989846874f",
            Name = "Test Recipe 1",
            CoverImage = new ImageData
            {
                Url = "6461870173061120.webp",
                AltText = "A photo of test recipe 1"
            },
            CuisineId = 1,
            Description = "This is a test.",
            Created = new DateTime(638412046299055561, DateTimeKind.Utc),
            Ingredients =
            [
                "1 tsp of test ingredient 1",
                "1 cup of test ingredient 2"
            ],
            Instructions = new MarkdownData
            {
                Markdown = "This is a test.",
                Html = "<p>This is a test.</p>\n"
            }
        };
    }

    [Fact]
    public async void HandleAsyncSucceededForReadOperationWithAnonymousUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Read], new ClaimsPrincipal(), _recipe);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async void HandleAsyncSucceededForCreateOperationWithAuthenticatedUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Create], _owner, _recipe);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async void HandleAsyncFailedForCreateOperationWithAnonymousUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Create], new ClaimsPrincipal(), _recipe);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasFailed);
    }
}
