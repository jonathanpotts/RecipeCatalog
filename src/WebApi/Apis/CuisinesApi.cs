using IdGen;
using JonathanPotts.RecipeBook.WebApi.Data;
using JonathanPotts.RecipeBook.WebApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeBook.WebApi.Apis;

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

    public static async Task<Results<Created<CuisineDto>, ValidationProblem>> PostAsync(
        [AsParameters] Services services,
        CuisineDto dto)
    {
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add(nameof(dto.Name), ["Value is required."]);
        }

        if (errors.Count != 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        Cuisine cuisine = new()
        {
            Name = dto.Name
        };

        await services.Context.Cuisines.AddAsync(cuisine);
        await services.Context.SaveChangesAsync();

        return TypedResults.Created($"/api/v1/cuisines/{cuisine.Id}", new CuisineDto
        {
            Id = cuisine.Id,
            Name = cuisine.Name
        });
    }

    public static async Task<Results<Ok<CuisineDto>, ValidationProblem, NotFound>> PutAsync(
        [AsParameters] Services services,
        int id,
        CuisineDto dto)
    {
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add(nameof(dto.Name), ["Value is required."]);
        }

        if (errors.Count != 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var cuisine = await services.Context.Cuisines
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cuisine == null)
        {
            return TypedResults.NotFound();
        }

        services.Context.Entry(cuisine).CurrentValues.SetValues(dto);
        await services.Context.SaveChangesAsync();

        return TypedResults.Ok(new CuisineDto
        {
            Id = cuisine.Id,
            Name = cuisine.Name
        });
    }

    public static async Task<Results<NoContent, NotFound>> DeleteAsync(
        [AsParameters] Services services,
        int id)
    {
        var cuisine = await services.Context.Cuisines
            .FirstOrDefaultAsync(x => x.Id == id);

        if (cuisine == null)
        {
            return TypedResults.NotFound();
        }

        services.Context.Cuisines.Remove(cuisine);
        await services.Context.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public class Services(
        ApplicationDbContext context,
        IdGenerator idGenerator)
    {
        public ApplicationDbContext Context { get; set; } = context;

        public IdGenerator IdGenerator { get; } = idGenerator;
    }
}
