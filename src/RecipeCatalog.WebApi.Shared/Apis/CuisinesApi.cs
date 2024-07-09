using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RecipeCatalog.Application.Contracts.Models;
using RecipeCatalog.Application.Contracts.Services;
using RecipeCatalog.Application.Validation;

namespace RecipeCatalog.WebApi.Shared.Apis;

public static class CuisinesApi
{
    public static IEndpointRouteBuilder MapCuisinesApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/cuisines")
            .WithTags("Cuisines");

        group.MapGet("/", GetListAsync);
        group.MapGet("/{id:int}", GetAsync);
        group.MapPost("/", PostAsync);
        group.MapPut("/{id:int}", PutAsync);
        group.MapDelete("/{id:int}", DeleteAsync);

        return builder;
    }

    public static async Task<Ok<IEnumerable<CuisineDto>>> GetListAsync(
        ICuisineService cuisineService,
        CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await cuisineService.GetListAsync(cancellationToken));
    }

    public static async Task<Results<Ok<CuisineDto>, NotFound>> GetAsync(
        ICuisineService cuisineService,
        int id,
        CancellationToken cancellationToken = default)
    {
        var cuisine = await cuisineService.GetAsync(id, cancellationToken);

        if (cuisine == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(cuisine);
    }

    [Authorize]
    public static async Task<Results<Created<CuisineDto>, ValidationProblem, ForbidHttpResult>> PostAsync(
        ICuisineService cuisineService,
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            var cuisine = await cuisineService.CreateAsync(
                dto,
                user,
                cancellationToken);

            return TypedResults.Created($"/api/v1/cuisines/{cuisine.Id}", cuisine);
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
    public static async Task<Results<Ok<CuisineDto>, ValidationProblem, NotFound, ForbidHttpResult>> PutAsync(
        ICuisineService cuisineService,
        int id,
        CreateUpdateCuisineDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            var cuisine = await cuisineService.UpdateAsync(
                id,
                dto,
                user,
                cancellationToken);

            if (cuisine == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(cuisine);
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
        ICuisineService cuisineService,
        int id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await cuisineService.DeleteAsync(id, user, cancellationToken))
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
}
