using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IdGen;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;
using JonathanPotts.RecipeCatalog.WebApi.Authorization;
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
        [Range(0, int.MaxValue)] int? skip,
        [Range(1, IRecipesService.MaxItemsPerPage)] int? take,
        int[]? cuisineIds,
        bool? withDetails,
        CancellationToken cancellationToken)
    {
        try
        {
            return TypedResults.Ok(
                await services.RecipesService.GetPagedResultAsync(
                    skip,
                    take,
                    cuisineIds,
                    withDetails,
                    cancellationToken));
        }
        catch (FluentValidation.ValidationException ex)
        {
            return TypedResults.ValidationProblem(
                ex.Errors.GroupBy(x => x.PropertyName).ToDictionary(
                    x => x.Key,
                    x => x.Select(x => x.ErrorMessage).ToArray()));
        }
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
        IRecipesService recipesService,
        RecipeCatalogDbContext context,
        IdGenerator idGenerator,
        IAuthorizationService authorizationService,
        UserManager<User> userManager)
    {
        public IRecipesService RecipesService { get; } = recipesService;

        public RecipeCatalogDbContext Context { get; } = context;

        public IdGenerator IdGenerator { get; } = idGenerator;

        public IAuthorizationService AuthorizationService { get; } = authorizationService;

        public UserManager<User> UserManager { get; } = userManager;
    }
}
