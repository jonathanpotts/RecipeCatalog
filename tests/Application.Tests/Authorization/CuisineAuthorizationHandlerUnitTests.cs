using System.Security.Claims;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Authorization;

public class CuisineAuthorizationHandlerUnitTests
{

    private readonly CuisineAuthorizationHandler _handler;

    private readonly Cuisine _cuisine = new()
    {
        Id = 1,
        Name = "Test"
    };

    private readonly ClaimsPrincipal _admin = new(new ClaimsIdentity(
        [
            new (ClaimTypes.NameIdentifier, "73edf737-df51-4c06-ac6f-3ec6d79f1f12")
        ], "Test"));

    private readonly ClaimsPrincipal _user = new(new ClaimsIdentity(
    [
        new (ClaimTypes.NameIdentifier, "4f4990ff-1f93-4ba8-a36d-c2833d476c7d")
    ], "Test"));

    public CuisineAuthorizationHandlerUnitTests()
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
            .Setup(x => x.IsInRoleAsync(It.Is<User>(x => x.Id == "73edf737-df51-4c06-ac6f-3ec6d79f1f12"), "Administrator").Result)
            .Returns(true);
        userManagerMock
            .Setup(x => x.IsInRoleAsync(It.Is<User>(x => x.Id != "73edf737-df51-4c06-ac6f-3ec6d79f1f12"), "Administrator").Result)
            .Returns(false);

        _handler = new(userManagerMock.Object);
    }

    [Fact]
    public async void HandleAsyncSucceededForReadOperationWithAnonymousUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Read], new ClaimsPrincipal(), _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async void HandleAsyncSucceededForCreateOperationWithAuthenticatedUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Create], _user, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async void HandleAsyncFailedForCreateOperationWithAnonymousUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Create], new ClaimsPrincipal(), _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasFailed);
    }

    [Fact]
    public async void HandleAsyncSucceededForUpdateOperationWithAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Update], _admin, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async void HandleAsyncFailedForUpdateOperationWithNonOwnerNonAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Update], _user, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasFailed);
    }

    [Fact]
    public async void HandleAsyncSucceededForDeleteOperationWithAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Delete], _admin, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async void HandleAsyncFailedForDeleteOperationWithNonOwnerNonAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Delete], _user, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasFailed);
    }
}
