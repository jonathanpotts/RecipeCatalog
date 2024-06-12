using System.Security.Claims;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Tests.Shared;
using Microsoft.AspNetCore.Authorization;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Authorization;

public class CuisineAuthorizationHandlerUnitTests
{
    private readonly CuisineAuthorizationHandler _handler = new(Mocks.CreateUserManagerMock().Object);
    private readonly Cuisine _cuisine = TestData.Cuisines[0];
    private readonly ClaimsPrincipal _admin = TestData.GetAdministrator();
    private readonly ClaimsPrincipal _user = TestData.GetUser();

    [Fact]
    public async Task HandleAsyncSucceededForReadOperationWithAnonymousUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Read], new ClaimsPrincipal(), _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsyncSucceededForCreateOperationWithAuthenticatedUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Create], _user, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsyncFailedForCreateOperationWithAnonymousUser()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Create], new ClaimsPrincipal(), _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasFailed);
    }

    [Fact]
    public async Task HandleAsyncSucceededForUpdateOperationWithAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Update], _admin, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsyncFailedForUpdateOperationWithNonOwnerNonAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Update], _user, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasFailed);
    }

    [Fact]
    public async Task HandleAsyncSucceededForDeleteOperationWithAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Delete], _admin, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsyncFailedForDeleteOperationWithNonOwnerNonAdmin()
    {
        // Arrange
        AuthorizationHandlerContext context = new([Operations.Delete], _user, _cuisine);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasFailed);
    }
}
