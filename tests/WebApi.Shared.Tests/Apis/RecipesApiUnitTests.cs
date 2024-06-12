using System.Security.Claims;
using FluentValidation.Results;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.WebApi.Shared.Apis;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace JonathanPotts.RecipeCatalog.WebApi.Shared.Tests.Apis;

public sealed class RecipesApiUnitTests
{
    [Fact]
    public async Task GetListAsyncReturnsOk()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.GetListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int[]?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new PagedResult<RecipeWithCuisineDto>(0, []));

        // Act
        var result = await RecipesApi.GetListAsync(recipeServiceMock.Object, null, null, null, null, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
    }

    [Fact]
    public async Task GetListAsyncReturnsValidationProblemWhenValidationExceptionThrown()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.GetListAsync(It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<int[]?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new FluentValidation.ValidationException([new ValidationFailure("test", "Test failure")]));

        // Act
        var result = await RecipesApi.GetListAsync(recipeServiceMock.Object, null, null, null, null, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task GetAsyncReturnsOk()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.GetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new RecipeWithCuisineDto());

        // Act
        var result = await RecipesApi.GetAsync(recipeServiceMock.Object, default, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<RecipeWithCuisineDto>>(result.Result);
    }

    [Fact]
    public async Task GetAsyncReturnsNotFoundWhenNullReturned()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.GetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()).Result)
            .Returns((RecipeWithCuisineDto?)null);

        // Act
        var result = await RecipesApi.GetAsync(recipeServiceMock.Object, default, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GetCoverImageAsyncReturnsPhysicalFile()
    {
        // Arrange
        var file = Path.GetTempFileName();

        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.GetCoverImageAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()).Result)
            .Returns(file);

        // Act
        var result = await RecipesApi.GetCoverImageAsync(recipeServiceMock.Object, default, default);
        File.Delete(file);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PhysicalFileHttpResult>(result.Result);
    }

    [Fact]
    public async Task GetCoverImageAsyncReturnsNotFoundWhenRecordNotFound()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.GetCoverImageAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()).Result)
            .Returns((string?)null);

        // Act
        var result = await RecipesApi.GetCoverImageAsync(recipeServiceMock.Object, default, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task PostAsyncReturnsCreated()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateUpdateRecipeDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new RecipeWithCuisineDto());

        // Act
        var result = await RecipesApi.PostAsync(recipeServiceMock.Object, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Created<RecipeWithCuisineDto>>(result.Result);
    }

    [Fact]
    public async Task PostAsyncReturnsValidationProblemWhenValidationExceptionThrown()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateUpdateRecipeDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new FluentValidation.ValidationException([new ValidationFailure("test", "Test failure")]));

        // Act
        var result = await RecipesApi.PostAsync(recipeServiceMock.Object, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task PostAsyncReturnsForbidWhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<CreateUpdateRecipeDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new UnauthorizedAccessException());

        // Act
        var result = await RecipesApi.PostAsync(recipeServiceMock.Object, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsOk()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<CreateUpdateRecipeDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new RecipeWithCuisineDto());

        // Act
        var result = await RecipesApi.PutAsync(recipeServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<RecipeWithCuisineDto>>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsNotFoundWhenRecordNotFound()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<CreateUpdateRecipeDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns((RecipeWithCuisineDto?)null);

        // Act
        var result = await RecipesApi.PutAsync(recipeServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsValidationProblemWhenValidationExceptionThrown()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<CreateUpdateRecipeDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new FluentValidation.ValidationException([new ValidationFailure("test", "Test failure")]));

        // Act
        var result = await RecipesApi.PutAsync(recipeServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationProblem>(result.Result);
    }

    [Fact]
    public async Task PutAsyncReturnsForbidWhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.UpdateAsync(It.IsAny<long>(), It.IsAny<CreateUpdateRecipeDto>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new UnauthorizedAccessException());

        // Act
        var result = await RecipesApi.PutAsync(recipeServiceMock.Object, default, new(), new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsyncReturnsNoContent()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<long>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(true);

        // Act
        var result = await RecipesApi.DeleteAsync(recipeServiceMock.Object, default, new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task DeleteAsyncReturnsNotFoundWhenRecordNotFound()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<long>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Returns(false);

        // Act
        var result = await RecipesApi.DeleteAsync(recipeServiceMock.Object, default, new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task DeleteAsyncReturnsForbidWhenUnauthorizedAccessExceptionThrown()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.DeleteAsync(It.IsAny<long>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new UnauthorizedAccessException());

        // Act
        var result = await RecipesApi.DeleteAsync(recipeServiceMock.Object, default, new(), default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ForbidHttpResult>(result.Result);
    }

    [Fact]
    public async Task SearchAsyncReturnsOk()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new PagedResult<RecipeWithCuisineDto>(0, []));

        // Act
        var result = await RecipesApi.SearchAsync(recipeServiceMock.Object, string.Empty, null, null, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Ok<PagedResult<RecipeWithCuisineDto>>>(result.Result);
    }

    [Fact]
    public async Task SearchAsyncReturnsValidationProblemWhenValidationExceptionThrown()
    {
        // Arrange
        Mock<IRecipeService> recipeServiceMock = new();
        recipeServiceMock
            .Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()).Result)
            .Throws(new FluentValidation.ValidationException([new ValidationFailure("test", "Test failure")]));

        // Act
        var result = await RecipesApi.SearchAsync(recipeServiceMock.Object, string.Empty, null, null, default);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ValidationProblem>(result.Result);
    }
}
