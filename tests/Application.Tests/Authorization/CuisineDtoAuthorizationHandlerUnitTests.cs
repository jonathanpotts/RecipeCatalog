﻿using System.Security.Claims;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Tests.Shared;
using Microsoft.AspNetCore.Authorization;

namespace JonathanPotts.RecipeCatalog.Application.Tests.Authorization;

public class CuisineDtoAuthorizationHandlerUnitTests
{
    private readonly CuisineDtoAuthorizationHandler _handler = new(Mocks.CreateUserManagerMock().Object);
    private readonly CuisineDto _cuisine = TestData.Cuisines[0].ToCuisineDto();
    private readonly ClaimsPrincipal _admin = TestData.GetAdministrator();
    private readonly ClaimsPrincipal _user = TestData.GetUser();

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
