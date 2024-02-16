using System.Security;
using System.Security.Claims;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace JonathanPotts.RecipeCatalog.WebApi.Apis;

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
        catch (SecurityException)
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
            return TypedResults.Ok(await cuisineService.UpdateAsync(
                id,
                dto,
                user,
                cancellationToken));
        }
        catch (FluentValidation.ValidationException ex)
        {
            return TypedResults.ValidationProblem(ex.ToDictionary());
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (SecurityException)
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
            await cuisineService.DeleteAsync(id, user, cancellationToken);

            return TypedResults.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return TypedResults.NotFound();
        }
        catch (SecurityException)
        {
            return TypedResults.Forbid();
        }
    }
}
