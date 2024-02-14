using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IdGen;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.WebApi.Authorization;
using JonathanPotts.RecipeCatalog.WebApi.Data;
using JonathanPotts.RecipeCatalog.WebApi.Models;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace JonathanPotts.RecipeCatalog.WebApi.Apis;

public static class RecipesApi
{
    private const int MaxItemsPerPage = 50;
    private const int DefaultItemsPerPage = 20;

    private static readonly string s_imagesDirectory = Path.Combine(AppContext.BaseDirectory, "Images");

    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()
        .UseReferralLinks(["ugc"])
        .Build();

    public static IEndpointRouteBuilder MapRecipesApi(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api/v1/recipes")
            .AddFluentValidationAutoValidation()
            .WithTags("Recipes");

        group.MapGet("/", GetListAsync);
        group.MapGet("/{id:long}", GetAsync);
        group.MapGet("/{id:long}/coverImage", GetCoverImageAsync);
        group.MapPost("/", PostAsync);
        group.MapPut("/{id:long}", PutAsync);
        group.MapDelete("/{id:long}", DeleteAsync);

        return builder;
    }

    public static async Task<Results<Ok<PagedResult<RecipeWithCuisineDto>>, ValidationProblem>> GetListAsync(
        [AsParameters] Services services,
        [Range(1, MaxItemsPerPage)] int? top,
        long? last,
        int[]? cuisineIds,
        bool? withDetails)
    {
        if (top is < 1 or > MaxItemsPerPage)
        {
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                { nameof(top), [$"Value must be between 1 and {MaxItemsPerPage}."] }
            });
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
            OwnerId = x.OwnerId,
            Name = x.Name,
            CoverImage = x.CoverImage == null
                ? null
                : new ImageData
                {
                    Url = $"/api/v1/recipes/{x.Id}/coverImage",
                    AltText = x.CoverImage.AltText
                },
            Cuisine = x.Cuisine == null
                ? null
                : new CuisineDto
                {
                    Id = x.Cuisine.Id,
                    Name = x.Cuisine.Name
                },
            Description = withDetails.GetValueOrDefault(false) ? x.Description : null,
            Created = x.Created,
            Modified = x.Modified,
            Ingredients = withDetails.GetValueOrDefault(false) ? x.Ingredients : null,
            Instructions = withDetails.GetValueOrDefault(false) ? x.Instructions : null
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
            OwnerId = recipe.OwnerId,
            Name = recipe.Name,
            CoverImage = recipe.CoverImage == null
                ? null
                : new ImageData
                {
                    Url = $"/api/v1/recipes/{recipe.Id}/coverImage",
                    AltText = recipe.CoverImage.AltText
                },
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

    [SwaggerResponse(200, contentTypes: "image/webp")]
    public static async Task<Results<PhysicalFileHttpResult, NotFound>> GetCoverImageAsync(
        [AsParameters] Services services,
        long id)
    {
        var recipe = await services.Context.Recipes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe?.CoverImage?.Url == null)
        {
            return TypedResults.NotFound();
        }

        var imagePath = Path.Combine(s_imagesDirectory, recipe.CoverImage.Url);
        var lastModified = File.GetLastWriteTimeUtc(imagePath);

        return TypedResults.PhysicalFile(imagePath, "image/webp", lastModified: lastModified);
    }

    [Authorize]
    public static async Task<Results<Created<RecipeWithCuisineDto>, ForbidHttpResult>> PostAsync(
        [AsParameters] Services services,
        ClaimsPrincipal user,
        RecipeCreateOrUpdateDto dto)
    {
        Recipe recipe = new()
        {
            Id = services.IdGenerator.CreateId(),
            OwnerId = services.UserManager.GetUserId(user),
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

        var authResult = await services.AuthorizationService.AuthorizeAsync(user, recipe, Operations.Delete);

        if (!authResult.Succeeded)
        {
            return TypedResults.Forbid();
        }

        await services.Context.Recipes.AddAsync(recipe);
        await services.Context.SaveChangesAsync();

        return TypedResults.Created($"/api/v1/recipes/{recipe.Id}", new RecipeWithCuisineDto
        {
            Id = recipe.Id,
            OwnerId = recipe.OwnerId,
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

    [Authorize]
    public static async Task<Results<Ok<RecipeWithCuisineDto>, NotFound, ForbidHttpResult>> PutAsync(
        [AsParameters] Services services,
        ClaimsPrincipal user,
        long id,
        RecipeCreateOrUpdateDto dto)
    {
        var recipe = await services.Context.Recipes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return TypedResults.NotFound();
        }

        var authResult = await services.AuthorizationService.AuthorizeAsync(user, recipe, Operations.Update);

        if (!authResult.Succeeded)
        {
            return TypedResults.Forbid();
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
            OwnerId = recipe.OwnerId,
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

    [Authorize]
    public static async Task<Results<NoContent, NotFound, ForbidHttpResult>> DeleteAsync(
        [AsParameters] Services services,
        ClaimsPrincipal user,
        long id)
    {
        var recipe = await services.Context.Recipes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (recipe == null)
        {
            return TypedResults.NotFound();
        }

        var authResult = await services.AuthorizationService.AuthorizeAsync(user, recipe, Operations.Delete);

        if (!authResult.Succeeded)
        {
            return TypedResults.Forbid();
        }

        services.Context.Recipes.Remove(recipe);
        await services.Context.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public class Services(
        ApplicationDbContext context,
        IdGenerator idGenerator,
        IAuthorizationService authorizationService,
        UserManager<ApplicationUser> userManager)
    {
        public ApplicationDbContext Context { get; set; } = context;

        public IdGenerator IdGenerator { get; } = idGenerator;

        public IAuthorizationService AuthorizationService { get; } = authorizationService;

        public UserManager<ApplicationUser> UserManager { get; } = userManager;
    }
}
