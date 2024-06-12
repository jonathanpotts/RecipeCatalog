using System.Security.Claims;
using FluentValidation.Results;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.WebApi.Shared.Apis;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace JonathanPotts.RecipeCatalog.WebApi.Shared.Tests.Apis;

public sealed class CuisinesApiUnitTests
{
    [Fact]
    public async Task GetListAsyncReturnsOk()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.GetListAsync(It.IsAny<CancellationToken>()).Result)
            .Returns([]);

        // Act
        var result = await CuisinesApi.GetListAsync(cuisineServiceMock.Object, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<IEnumerable<CuisineDto>>>(result);
    }

    [Fact]
    public async Task GetAsyncReturnsOk()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new CuisineDto());

        // Act
        var result = await CuisinesApi.GetAsync(cuisineServiceMock.Object, default, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<CuisineDto>>(result.Result);
    }

    [Fact]
    public async Task GetAsyncReturnsNotFoundWhenNullReturned()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()).Result)
            .Returns((CuisineDto?)null);

        // Act
        var result = await CuisinesApi.GetAsync(cuisineServiceMock.Object, default, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task PostAsyncReturnsCreated()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateUpdateCuisineDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new CuisineDto());

        // Act
        var result = await CuisinesApi.PostAsync(cuisineServiceMock.Object, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Created<CuisineDto>>(result.Result);
    }

    [Fact]
    public async Task PostAsyncReturnsValidationProblemWhenValidationExceptionThrown()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateUpdateCuisineDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new FluentValidation.ValidationException([new ValidationFailure("test", "Test failure")]));

        // Act
        var result = await CuisinesApi.PostAsync(cuisineServiceMock.Object, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task PostAsyncReturnsForbidWhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateUpdateCuisineDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new UnauthorizedAccessException());

        // Act
        var result = await CuisinesApi.PostAsync(cuisineServiceMock.Object, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsOk()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<CreateUpdateCuisineDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new CuisineDto());

        // Act
        var result = await CuisinesApi.PutAsync(cuisineServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<CuisineDto>>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsNotFoundWhenRecordNotFound()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<CreateUpdateCuisineDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns((CuisineDto?)null);

        // Act
        var result = await CuisinesApi.PutAsync(cuisineServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsValidationProblemWhenValidationExceptionThrown()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<CreateUpdateCuisineDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new FluentValidation.ValidationException([new ValidationFailure("test", "Test failure")]));

        // Act
        var result = await CuisinesApi.PutAsync(cuisineServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsForbidWhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<CreateUpdateCuisineDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new UnauthorizedAccessException());

        // Act
        var result = await CuisinesApi.PutAsync(cuisineServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsyncReturnsNoContent()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(true);

        // Act
        var result = await CuisinesApi.DeleteAsync(cuisineServiceMock.Object, default, new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task DeleteAsyncReturnsNotFoundWhenRecordNotFound()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(false);

        // Act
        var result = await CuisinesApi.DeleteAsync(cuisineServiceMock.Object, default, new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task DeleteAsyncReturnsForbidWhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        Mock<ICuisineService> cuisineServiceMock = new();
        cuisineServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new UnauthorizedAccessException());

        // Act
        var result = await CuisinesApi.DeleteAsync(cuisineServiceMock.Object, default, new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ForbidHttpResult>(result.Result);
    }
}
