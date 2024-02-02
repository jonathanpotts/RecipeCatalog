using System.Security.Claims;
using JonathanPotts.RecipeCatalog.WebApi.Authorization;
using JonathanPotts.RecipeCatalog.WebApi.Data;
using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace JonathanPotts.RecipeCatalog.WebApi.Apis;

public static class CuisinesApi
{
    public static IEndpointRouteBuilder MapCuisinesApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/cuisines")
            .AddFluentValidationAutoValidation()
            .WithTags("Cuisines");

        group.MapGet("/", GetListAsync);
        group.MapGet("/{id:int}", GetAsync);
        group.MapPost("/", PostAsync);
        group.MapPut("/{id:int}", PutAsync);
        group.MapDelete("/{id:int}", DeleteAsync);

        return builder;
    }

    public static async Task<Ok<CuisineDto[]>> GetListAsync(
        [AsParameters] Services services)
    {
        var cuisines = services.Context.Cuisines.AsNoTracking();

        cuisines = cuisines
            .OrderBy(x => x.Name);

        var items = cuisines.Select(x => new CuisineDto
        {
            Id = x.Id,
            Name = x.Name
        });

        return TypedResults.Ok(await items.ToArrayAsync());
    }

    public static async Task<Results<Ok<CuisineDto>, NotFound>> GetAsync(
        [AsParameters] Services services,
        int id)
    {
        var cuisine = await services.Context.Cuisines.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cuisine == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new CuisineDto
        {
            Id = cuisine.Id,
            Name = cuisine.Name
        });
    }

    [Authorize]
    public static async Task<Results<Created<CuisineDto>, ForbidHttpResult>> PostAsync(
        [AsParameters] Services services,
        ClaimsPrincipal user,
        CuisineDto dto)
    {
        Cuisine cuisine = new()
        {
            Name = dto.Name
        };

        var authResult = await services.AuthorizationService.AuthorizeAsync(user, cuisine, Operations.Create);

        if (!authResult.Succeeded)
        {
            return TypedResults.Forbid();
        }

        await services.Context.Cuisines.AddAsync(cuisine);
        await services.Context.SaveChangesAsync();

        return TypedResults.Created($"/api/v1/cuisines/{cuisine.Id}", new CuisineDto
        {
            Id = cuisine.Id,
            Name = cuisine.Name
        });
    }

    [Authorize]
    public static async Task<Results<Ok<CuisineDto>, NotFound, ForbidHttpResult>> PutAsync(
        [AsParameters] Services services,
        ClaimsPrincipal user,
        int id,
        CuisineDto dto)
    {
        var cuisine = await services.Context.Cuisines
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cuisine == null)
        {
            return TypedResults.NotFound();
        }

        var authResult = await services.AuthorizationService.AuthorizeAsync(user, cuisine, Operations.Update);

        if (!authResult.Succeeded)
        {
            return TypedResults.Forbid();
        }

        services.Context.Entry(cuisine).CurrentValues.SetValues(dto);
        await services.Context.SaveChangesAsync();

        return TypedResults.Ok(new CuisineDto
        {
            Id = cuisine.Id,
            Name = cuisine.Name
        });
    }

    [Authorize]
    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteAsync(
        [AsParameters] Services services,
        ClaimsPrincipal user,
        int id)
    {
        var cuisine = await services.Context.Cuisines
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cuisine == null)
        {
            return TypedResults.NotFound();
        }

        var authResult = await services.AuthorizationService.AuthorizeAsync(user, cuisine, Operations.Delete);

        if (!authResult.Succeeded)
        {
            return TypedResults.Forbid();
        }

        services.Context.Cuisines.Remove(cuisine);
        await services.Context.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public class Services(
        ApplicationDbContext context,
        IAuthorizationService authorizationService)
    {
        public ApplicationDbContext Context { get; set; } = context;

        public IAuthorizationService AuthorizationService { get; } = authorizationService;
    }
}
