using System.Numerics.Tensors;
using System.Security;
using System.Security.Claims;
using FluentValidation;
using IdGen;
using JonathanPotts.RecipeCatalog.AI;
using JonathanPotts.RecipeCatalog.Application.Authorization;
using JonathanPotts.RecipeCatalog.Application.Contracts.Models;
using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.Application.Mapping;
using JonathanPotts.RecipeCatalog.Application.Validation;
using JonathanPotts.RecipeCatalog.Domain;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace JonathanPotts.RecipeCatalog.Application.Services;

public class RecipeService(
    RecipeCatalogDbContext context,
    IIdGenerator<long> idGenerator,
    UserManager<User> userManager,
    IAuthorizationService authorizationService,
    IServiceProvider serviceProvider)
    : IRecipeService
{
    private const int DefaultItemsPerPage = 20;
    private const float DistanceThreshold = 0.6f;
    private const int MaxImageDimension = 1024;
    private const int ImageQuality = 80;
    private const SKFilterQuality ImageFilterQuality = SKFilterQuality.High;

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

    public async Task UpdateCoverImageAsync(
        long id,
        Stream imageData,
        string? description,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var recipe = await context.Recipes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
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

        await Task.Run(async () =>
        {
            SKBitmap bitmap = SKBitmap.Decode(imageData)
                ?? throw new ArgumentException("The image could not be decoded.", nameof(imageData));

            try
            {
                if (bitmap.Width > MaxImageDimension || bitmap.Height > MaxImageDimension)
                {
                    var scaleFactor = (double)MaxImageDimension / Math.Max(bitmap.Width, bitmap.Height);
                    var width = (int)Math.Floor(bitmap.Width * scaleFactor);
                    var height = (int)Math.Floor(bitmap.Height * scaleFactor);

                    var resizedBitmap = bitmap.Resize(new SKImageInfo(width, height), ImageFilterQuality);

                    bitmap.Dispose();
                    bitmap = resizedBitmap;
                }

                using var data = bitmap.Encode(SKEncodedImageFormat.Webp, ImageQuality);

                await File.WriteAllBytesAsync(
                    Path.Combine(s_imagesDirectory, $"{id}.webp"),
                    data.AsSpan().ToArray(),
                    cancellationToken);
            }
            finally
            {
                bitmap.Dispose();
            }
        },
        cancellationToken);

        recipe.CoverImage ??= new();
        recipe.CoverImage.Url = $"{id}.webp";
        recipe.CoverImage.AltText = description;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCoverImageAsync(
        long id,
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
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

        var coverImage = recipe.CoverImage?.Url
            ?? throw new KeyNotFoundException();

        recipe.CoverImage = null;

        await context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(coverImage))
        {
            File.Delete(Path.Combine(s_imagesDirectory, coverImage));
        }
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

        var textGenerator = serviceProvider.GetService<IAITextGenerator>();

        if (textGenerator != null)
        {
            var nameEmbeddings = await textGenerator.GenerateEmbeddingsAsync(
                recipe.Name!.Trim().ReplaceLineEndings().Replace(Environment.NewLine, " ").ToLower(),
                cancellationToken);
            recipe.NameEmbeddings = nameEmbeddings.ToArray();

            if (!string.IsNullOrEmpty(recipe.Description))
            {
                var descriptionEmbeddings = await textGenerator.GenerateEmbeddingsAsync(
                    recipe.Description.Trim().ReplaceLineEndings().Replace(Environment.NewLine, " ").ToLower(),
                    cancellationToken);
                recipe.DescriptionEmbeddings = descriptionEmbeddings.ToArray();
            }
        }

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

        recipe.Instructions!.Markdown = dto.Instructions;
        recipe.Instructions!.Html = Markdown.ToHtml(dto.Instructions!, s_pipeline);

        recipe.Modified = DateTime.UtcNow;

        var textGenerator = serviceProvider.GetService<IAITextGenerator>();

        if (textGenerator != null)
        {
            var nameEmbeddings = await textGenerator.GenerateEmbeddingsAsync(
                recipe.Name!.Trim().ReplaceLineEndings().Replace(Environment.NewLine, " ").ToLower(),
                cancellationToken);
            recipe.NameEmbeddings = nameEmbeddings.ToArray();

            if (!string.IsNullOrEmpty(recipe.Description))
            {
                var descriptionEmbeddings = await textGenerator.GenerateEmbeddingsAsync(
                    recipe.Description.Trim().ReplaceLineEndings().Replace(Environment.NewLine, " ").ToLower(),
                    cancellationToken);
                recipe.DescriptionEmbeddings = descriptionEmbeddings.ToArray();
            }
        }

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

        var coverImage = recipe.CoverImage?.Url;

        context.Recipes.Remove(recipe);
        await context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(coverImage))
        {
            var imagePath = Path.Combine(s_imagesDirectory, coverImage);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }

    public async Task<PagedResult<RecipeWithCuisineDto>> SearchAsync(
        string query,
        int? skip = null,
        int? take = null,
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

        var textGenerator = serviceProvider.GetService<IAITextGenerator>();

        if (textGenerator != null)
        {
            var queryEmbeddings = await textGenerator.GenerateEmbeddingsAsync(query.ToLower(), cancellationToken);

            // ideally would use a vector database such as pgvector to perform this search
            // this is an inefficient implementation using basic EF Core functionality
            Dictionary<long, float> distances = [];

            await foreach (var (id, nameEmbeddings, descriptionEmbeddings) in context.Recipes.AsNoTracking()
                .Select(x => Tuple.Create(x.Id, x.NameEmbeddings, x.DescriptionEmbeddings))
                .AsAsyncEnumerable().WithCancellation(cancellationToken))
            {
                var similarity = TensorPrimitives.CosineSimilarity(queryEmbeddings.Span, nameEmbeddings);
                var nameDistance = 1 - TensorPrimitives.CosineSimilarity(queryEmbeddings.Span, nameEmbeddings);
                var descriptionDistance = descriptionEmbeddings == null
                    ? 1.0f
                    : 1 - TensorPrimitives.CosineSimilarity(queryEmbeddings.Span, descriptionEmbeddings);
                var distance = Math.Min(nameDistance, descriptionDistance);

                distances.Add(id, distance);
            }

            distances = distances
                .Where(x => x.Value <= DistanceThreshold)
                .ToDictionary(x => x.Key, x => x.Value);

            List<Recipe> recipes = [];

            foreach (var (id, _) in distances.OrderBy(x => x.Value).Skip(skip.Value).Take(take.Value))
            {
                var recipe = await context.Recipes.AsNoTracking()
                    .Include(x => x.Cuisine)
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (recipe != null)
                {
                    recipes.Add(recipe);
                }
            }

            return new PagedResult<RecipeWithCuisineDto>(
                distances.Count,
                recipes.Select(x => x.ToRecipeWithCuisineDto(false)));
        }
        else
        {
            var like = $"%{query}%";

            var results = context.Recipes
                .AsNoTracking()
                .Include(x => x.Cuisine)
                .Where(x =>
                    EF.Functions.Like(x.Name, like)
                    || EF.Functions.Like(x.Description, like));

            var count = await results.CountAsync(cancellationToken);

            var recipes = await results
                .OrderByDescending(x => x.Id)
                .Skip(skip.Value)
                .Take(take.Value)
                .ToArrayAsync(cancellationToken);

            return new PagedResult<RecipeWithCuisineDto>(
                count,
                recipes.Select(x => x.ToRecipeWithCuisineDto(false)));
        }
    }
}
