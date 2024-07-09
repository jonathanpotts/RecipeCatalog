using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RecipeCatalog.Application.Contracts.Models;
using RecipeCatalog.Application.Contracts.Services;
using RecipeCatalog.Application.Validation;
using Swashbuckle.AspNetCore.Annotations;

namespace RecipeCatalog.WebApi.Shared.Apis;

public static class RecipesApi
{
    public static IEndpointRouteBuilder MapRecipesApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/recipes")
            .WithTags("Recipes");

        group.MapGet("/", GetListAsync);
        group.MapGet("/{id:long}", GetAsync);
        group.MapGet("/{id:long}/coverImage", GetCoverImageAsync);
        group.MapPut("/{id:long}/coverImage", PutCoverImageAsync);
        group.MapDelete("/{id:long}/coverImage", DeleteCoverImageAsync);
        group.MapPost("/", PostAsync);
        group.MapPut("/{id:long}", PutAsync);
        group.MapDelete("/{id:long}", DeleteAsync);
        group.MapGet("/search", SearchAsync);

        return builder;
    }

    public static async Task<Results<Ok<PagedResult<RecipeWithCuisineDto>>, ValidationProblem>> GetListAsync(
        IRecipeService recipeService,
        [Range(0, int.MaxValue)] int? skip,
        [Range(1, IRecipeService.MaxItemsPerPage)] int? take,
        int[]? cuisineIds,
        bool? withDetails,
        CancellationToken cancellationToken)
    {
        try
        {
            var pagedResult = await recipeService.GetListAsync(
                skip,
                take,
                cuisineIds,
                withDetails,
                cancellationToken);

            return TypedResults.Ok(pagedResult);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return TypedResults.ValidationProblem(ex.ToDictionary());
        }
    }

    public static async Task<Results<Ok<RecipeWithCuisineDto>, NotFound>> GetAsync(
        IRecipeService recipeService,
        long id,
        CancellationToken cancellationToken)
    {
        var recipe = await recipeService.GetAsync(id, cancellationToken);

        if (recipe == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(recipe);
    }

    [SwaggerResponse(200, contentTypes: "image/webp")]
    public static async Task<Results<PhysicalFileHttpResult, NotFound>> GetCoverImageAsync(
        IRecipeService recipeService,
        long id,
        CancellationToken cancellationToken)
    {
        var coverImage = await recipeService.GetCoverImageAsync(id, cancellationToken);

        if (coverImage == null)
        {
            return TypedResults.NotFound();
        }

        var lastModified = File.GetLastWriteTimeUtc(coverImage);
        return TypedResults.PhysicalFile(coverImage, "image/webp", lastModified: lastModified);
    }

    [Authorize]
    public static async Task<Results<NoContent, NotFound, BadRequest, ForbidHttpResult>> PutCoverImageAsync(
        IRecipeService recipeService,
        long id,
        IFormFile imageFile,
        string? description,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            using var imageData = imageFile.OpenReadStream();

            if (!await recipeService.UpdateCoverImageAsync(
                id,
                imageData,
                description,
                user,
                cancellationToken))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        }
        catch (ArgumentException)
        {
            return TypedResults.BadRequest();
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    [Authorize]
    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteCoverImageAsync(
    IRecipeService recipeService,
    long id,
    ClaimsPrincipal user,
    CancellationToken cancellationToken)
    {
        try
        {
            if (!await recipeService.DeleteCoverImageAsync(
                id,
                user,
                cancellationToken))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    [Authorize]
    public static async Task<Results<Created<RecipeWithCuisineDto>, ValidationProblem, ForbidHttpResult>> PostAsync(
        IRecipeService recipeService,
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await recipeService.CreateAsync(
                dto,
                user,
                cancellationToken);

            return TypedResults.Created($"/api/v1/recipes/{recipe.Id}", recipe);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return TypedResults.ValidationProblem(ex.ToDictionary());
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    [Authorize]
    public static async Task<Results<Ok<RecipeWithCuisineDto>, ValidationProblem, NotFound, ForbidHttpResult>> PutAsync(
        IRecipeService recipeService,
        long id,
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            var recipe = await recipeService.UpdateAsync(
                id,
                dto,
                user,
                cancellationToken);

            if (recipe == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(recipe);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return TypedResults.ValidationProblem(ex.ToDictionary());
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    [Authorize]
    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteAsync(
        IRecipeService recipeService,
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await recipeService.DeleteAsync(id, user, cancellationToken))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }

    public static async Task<Results<Ok<PagedResult<RecipeWithCuisineDto>>, ValidationProblem>> SearchAsync(
        IRecipeService recipeService,
        string query,
        [Range(0, int.MaxValue)] int? skip,
        [Range(1, IRecipeService.MaxItemsPerPage)] int? take,
        CancellationToken cancellationToken)
    {
        try
        {
            return TypedResults.Ok(await recipeService.SearchAsync(query, skip, take, cancellationToken));
        }
        catch (FluentValidation.ValidationException ex)
        {
            return TypedResults.ValidationProblem(ex.ToDictionary());
        }
    }
}
