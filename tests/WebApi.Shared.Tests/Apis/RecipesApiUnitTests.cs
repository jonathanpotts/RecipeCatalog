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
    public async void GetListAsyncReturnsOk()
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
    public async void GetListAsyncReturnsValidationProblemWhenValidationExceptionThrown()
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
    public async void GetAsyncReturnsOk()
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
    public async void GetAsyncReturnsNotFoundWhenNullReturned()
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
}
