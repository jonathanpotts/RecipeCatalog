using System.ComponentModel.DataAnnotations;
using IdGen;
using Markdig;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RecipeBook.Api.Data;
using RecipeBook.Api.Models;

namespace RecipeBook.Api.Apis;

public static class RecipesApi
{
    private const int MaxItemsPerPage = 50;
    private const int DefaultItemsPerPage = 20;

    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()
        .UseReferralLinks(["ugc"])
        .Build();

    public static IEndpointRouteBuilder MapRecipesApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/recipes")
            .WithTags("Recipes");

        group.MapGet("/", GetListAsync);
        group.MapGet("/{id:long}", GetAsync);
        group.MapPost("/", PostAsync);
        group.MapPut("/{id:long}", PutAsync);
        group.MapDelete("/{id:long}", DeleteAsync);

        return builder;
    }

    public static async Task<Results<Ok<PagedResult<RecipeWithCuisineDto>>, ValidationProblem>> GetListAsync(
        [AsParameters] Services services,
        [Range(1, MaxItemsPerPage)] int? top,
        long? last,
        int[]? cuisineIds)
    {
        if (top is < 1 or > MaxItemsPerPage)
        {
            Dictionary<string, string[]> errors = new()
            {
                { nameof(top), [$"Value must be between 1 and {MaxItemsPerPage}."] }
            };

            return TypedResults.ValidationProblem(errors);
        }

        IQueryable<Recipe> recipes = services.Context.Recipes;

        if (cuisineIds?.Length > 0)
        {
            recipes = recipes.Where(x => cuisineIds.Contains(x.CuisineId));
        }

        var total = await recipes.CountAsync();

        recipes = recipes.Include(x => x.Cuisine).AsNoTracking();

        if (last.HasValue)
        {
            recipes = recipes.Where(x => x.Id < last.Value);
        }

        recipes = recipes
            .OrderByDescending(x => x.Id)
            .Take(top.GetValueOrDefault(DefaultItemsPerPage));

        var items = recipes.Select(x => new RecipeWithCuisineDto
        {
            Id = x.Id,
            Name = x.Name,
            Cuisine = x.Cuisine == null
                ? null
                : new CuisineDto
                {
                    Id = x.Cuisine.Id,
                    Name = x.Cuisine.Name
                },
            Description = x.Description,
            Created = x.Created,
            Modified = x.Modified,
            Ingredients = x.Ingredients,
            Instructions = x.Instructions
        });

        return TypedResults.Ok(new PagedResult<RecipeWithCuisineDto>(total, await items.ToArrayAsync()));
    }

    public static async Task<Results<Ok<RecipeWithCuisineDto>, NotFound>> GetAsync(
        [AsParameters] Services services,
        long id)
    {
        var recipe = await services.Context.Recipes.Include(x => x.Cuisine).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new RecipeWithCuisineDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Cuisine = recipe.Cuisine == null
                ? null
                : new CuisineDto
                {
                    Id = recipe.Cuisine.Id,
                    Name = recipe.Cuisine.Name
                },
            Description = recipe.Description,
            Created = recipe.Created,
            Modified = recipe.Modified,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions
        });
    }

    public static async Task<Results<Created<RecipeWithCuisineDto>, ValidationProblem>> PostAsync(
        [AsParameters] Services services,
        RecipeCreateOrUpdateDto dto)
    {
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add(nameof(dto.Name), ["Value is required."]);
        }

        if (dto.Ingredients?.Length == 0 || (dto.Ingredients?.Any(string.IsNullOrWhiteSpace) ?? false))
        {
            errors.Add(nameof(dto.Ingredients), ["Value is required."]);
        }

        if (string.IsNullOrWhiteSpace(dto.Instructions))
        {
            errors.Add(nameof(dto.Instructions), ["Value is required."]);
        }

        if (errors.Count != 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        Recipe recipe = new()
        {
            Id = services.IdGenerator.CreateId(),
            Name = dto.Name,
            CuisineId = dto.CuisineId,
            Description = dto.Description,
            Created = DateTime.UtcNow,
            Ingredients = dto.Ingredients,
            Instructions = new MarkdownData
            {
                Markdown = dto.Instructions,
                Html = Markdown.ToHtml(dto.Instructions!, s_pipeline)
            }
        };

        await services.Context.Recipes.AddAsync(recipe);
        await services.Context.SaveChangesAsync();

        return TypedResults.Created($"/api/v1/recipes/{recipe.Id}", new RecipeWithCuisineDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Cuisine = recipe.Cuisine == null
                ? null
                : new CuisineDto
                {
                    Id = recipe.Cuisine.Id,
                    Name = recipe.Cuisine.Name
                },
            Description = recipe.Description,
            Created = recipe.Created,
            Modified = recipe.Modified,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions
        });
    }

    public static async Task<Results<Ok<RecipeWithCuisineDto>, ValidationProblem, NotFound>> PutAsync(
        [AsParameters] Services services,
        long id,
        RecipeCreateOrUpdateDto dto)
    {
        Dictionary<string, string[]> errors = [];

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            errors.Add(nameof(dto.Name), ["Value is required."]);
        }

        if (dto.Ingredients?.Length == 0 || (dto.Ingredients?.Any(string.IsNullOrWhiteSpace) ?? false))
        {
            errors.Add(nameof(dto.Ingredients), ["Value is required."]);
        }

        if (string.IsNullOrWhiteSpace(dto.Instructions))
        {
            errors.Add(nameof(dto.Instructions), ["Value is required."]);
        }

        if (errors.Count != 0)
        {
            return TypedResults.ValidationProblem(errors);
        }

        var recipe = await services.Context.Recipes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return TypedResults.NotFound();
        }

        services.Context.Entry(recipe).CurrentValues.SetValues(dto);

        recipe.Instructions ??= new MarkdownData();
        recipe.Instructions.Markdown = dto.Instructions;
        recipe.Instructions.Html = Markdown.ToHtml(dto.Instructions!, s_pipeline);

        recipe.Modified = DateTime.UtcNow;

        await services.Context.SaveChangesAsync();

        return TypedResults.Ok(new RecipeWithCuisineDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Cuisine = recipe.Cuisine == null
                ? null
                : new CuisineDto
                {
                    Id = recipe.Cuisine.Id,
                    Name = recipe.Cuisine.Name
                },
            Description = recipe.Description,
            Created = recipe.Created,
            Modified = recipe.Modified,
            Ingredients = recipe.Ingredients,
            Instructions = recipe.Instructions
        });
    }

    public static async Task<Results<NoContent, NotFound>> DeleteAsync(
        [AsParameters] Services services,
        long id)
    {
        var recipe = await services.Context.Recipes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return TypedResults.NotFound();
        }

        services.Context.Recipes.Remove(recipe);
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
