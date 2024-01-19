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

    public static async Task<Results<Ok<PagedResult<Recipe>>, ValidationProblem>> GetListAsync(
        [AsParameters] Services services,
        [Range(1, MaxItemsPerPage)] int? top,
        long? last)
    {
        if (top is < 1 or > MaxItemsPerPage)
        {
            Dictionary<string, string[]> errors = new()
            {
                { nameof(top), [$"Value must be between 1 and {MaxItemsPerPage}."] }
            };

            return TypedResults.ValidationProblem(errors);
        }

        var total = await services.Context.Recipes.CountAsync();

        var recipes = services.Context.Recipes.Include(x => x.Cuisine).AsNoTracking();

        if (last.HasValue)
        {
            recipes = recipes.Where(x => x.Id < last.Value);
        }

        recipes = recipes
            .OrderByDescending(x => x.Id)
            .Take(top.GetValueOrDefault(DefaultItemsPerPage));

        return TypedResults.Ok(new PagedResult<Recipe>(total, await recipes.ToArrayAsync()));
    }

    public static async Task<Results<Ok<Recipe>, NotFound>> GetAsync(
        [AsParameters] Services services,
        long id)
    {
        var recipe = await services.Context.Recipes.Include(x => x.Cuisine).AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(recipe);
    }

    public static async Task<Results<Created<Recipe>, ValidationProblem>> PostAsync(
        [AsParameters] Services services,
        CreateOrUpdateRecipeDto dto)
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

        return TypedResults.Created($"/api/v1/recipes/{recipe.Id}", recipe);
    }

    public static async Task<Results<Ok<Recipe>, ValidationProblem, NotFound>> PutAsync(
        [AsParameters] Services services,
        long id,
        CreateOrUpdateRecipeDto dto)
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

        recipe.Name = dto.Name;
        recipe.CuisineId = dto.CuisineId;
        recipe.Description = dto.Description;
        recipe.Ingredients = dto.Ingredients;

        recipe.Instructions ??= new MarkdownData();
        recipe.Instructions.Markdown = dto.Instructions;
        recipe.Instructions.Html = Markdown.ToHtml(dto.Instructions!, s_pipeline);

        recipe.Modified = DateTime.UtcNow;

        await services.Context.SaveChangesAsync();

        return TypedResults.Ok(recipe);
    }

    public static async Task<Results<NoContent, NotFound>> DeleteAsync(
        [AsParameters] Services services,
        long id)
    {
        var recipe = await services.Context.Recipes.FirstOrDefaultAsync(a => a.Id == id);

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
