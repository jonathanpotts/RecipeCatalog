using System.Security;
using System.Security.Claims;
using FluentValidation;
using IdGen;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Application.Validation;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JonathanPotts.RecipeCatalog.Application.Services;

public class RecipeService(
    RecipeCatalogDbContext context,
    IdGenerator idGenerator,
    UserManager<User> userManager,
    IAuthorizationService authorizationService)
    : IRecipeService
{
    private const int DefaultItemsPerPage = 20;

    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()
        .UseReferralLinks(["ugc"])
        .Build();

    private static readonly string s_imagesDirectory
        = Path.Combine(AppContext.BaseDirectory, "Images");

    public async Task<PagedResult<RecipeWithCuisineDto>> GetListAsync(
        int? skip = null,
        int? take = null,
        int[]? cuisineIds = null,
        bool? withDetails = null,
        CancellationToken cancellationToken = default)
    {
        if (skip != null)
        {
            InlineValidator<int> validator = [];
            validator.RuleFor(x => x).GreaterThanOrEqualTo(0);
            validator.ValidateAndThrow(skip.Value);
        }

        if (take != null)
        {
            InlineValidator<int> validator = [];
            validator.RuleFor(x => x)
                .GreaterThan(0)
                .LessThanOrEqualTo(IRecipeService.MaxItemsPerPage);
            validator.ValidateAndThrow(take.Value);
        }

        skip ??= 0;
        take ??= DefaultItemsPerPage;

        IQueryable<Recipe> queryable = context.Recipes
            .AsNoTracking()
            .Include(x => x.Cuisine);

        if (cuisineIds?.Length > 0)
        {
            queryable = queryable.Where(x => cuisineIds.Contains(x.CuisineId));
        }

        var count = await queryable.CountAsync(cancellationToken);

        var recipes = await queryable
            .OrderByDescending(x => x.Id)
            .Skip(skip.Value)
            .Take(take.Value)
            .ToArrayAsync(cancellationToken);

        return new PagedResult<RecipeWithCuisineDto>(
            count,
            recipes.Select(
                x => x.ToRecipeWithCuisineDto(withDetails.GetValueOrDefault(false))));
    }

    public async Task<RecipeWithCuisineDto?> GetAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var recipe = await context.Recipes
            .AsNoTracking()
            .Include(x => x.Cuisine)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return recipe?.ToRecipeWithCuisineDto();
    }

    public async Task<string> GetCoverImageAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var recipe = await context.Recipes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException();

        if (string.IsNullOrEmpty(recipe.CoverImage?.Url))
        {
            throw new KeyNotFoundException();
        }

        return Path.Combine(s_imagesDirectory, recipe.CoverImage.Url);
    }

    public async Task<RecipeWithCuisineDto> CreateAsync(
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        new CreateUpdateRecipeDtoValidator().ValidateAndThrow(dto);

        var recipe = dto.ToRecipe();

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            recipe,
            Operations.Create);

        if (!authResult.Succeeded)
        {
            throw new SecurityException(
                $"User is unauthorized to perform {nameof(Operations.Create)} operation on resource.");
        }

        recipe.Id = idGenerator.CreateId();
        recipe.OwnerId = userManager.GetUserId(user);
        recipe.Created = DateTime.UtcNow;
        recipe.Instructions!.Html = Markdown.ToHtml(
            recipe.Instructions.Markdown!,
            s_pipeline);

        await context.Recipes.AddAsync(recipe, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return recipe.ToRecipeWithCuisineDto();
    }

    public async Task<RecipeWithCuisineDto> UpdateAsync(
        long id,
        CreateUpdateRecipeDto dto,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        new CreateUpdateRecipeDtoValidator().ValidateAndThrow(dto);

        var recipe = await context.Recipes.FindAsync([id], cancellationToken)
            ?? throw new KeyNotFoundException();

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            recipe,
            Operations.Update);

        if (!authResult.Succeeded)
        {
            throw new SecurityException(
                $"User is unauthorized to perform {nameof(Operations.Update)} operation on resource.");
        }

        context.Entry(recipe).CurrentValues.SetValues(dto);

        recipe.Instructions ??= new MarkdownData();
        recipe.Instructions.Markdown = dto.Instructions;
        recipe.Instructions.Html = Markdown.ToHtml(dto.Instructions!, s_pipeline);

        recipe.Modified = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return recipe.ToRecipeWithCuisineDto();
    }

    public async Task DeleteAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var recipe = await context.Recipes.FindAsync([id], cancellationToken)
            ?? throw new KeyNotFoundException();

        var authResult = await authorizationService.AuthorizeAsync(
            user,
            recipe,
            Operations.Delete);

        if (!authResult.Succeeded)
        {
            throw new SecurityException(
                $"User is unauthorized to perform {nameof(Operations.Delete)} operation on resource.");
        }

        context.Recipes.Remove(recipe);
        await context.SaveChangesAsync(cancellationToken);
    }
}
