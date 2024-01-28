using System.Text.Json;
using JonathanPotts.RecipeCatalog.AIDataGenerator.Models;
using JonathanPotts.RecipeCatalog.AIDataGenerator.Services;
using JonathanPotts.RecipeCatalog.WebApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;
using Spectre.Console;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator;

internal class Worker(
    IOptions<WorkerOptions> options,
    IHostApplicationLifetime applicationLifetime,
    ILogger<Worker> logger,
    IAITextGenerator textGenerator,
    IAIImageGenerator imageGenerator) : BackgroundService
{
    private static readonly JsonSerializerOptions s_jsonOptions = new() { WriteIndented = true };

    private readonly IReadOnlyList<string> _cuisines = options.Value.Cuisines!.AsReadOnly();
    private readonly int _imageGenerationMaxConcurrency = options.Value.ImageGenerationMaxConcurrency ?? 1;
    private readonly int _imageQuality = options.Value.ImageQuality ?? 60;
    private readonly int _recipeGenerationMaxConcurrency = options.Value.RecipeGenerationMaxConcurrency ?? 5;
    private readonly int _recipesPerCuisine = options.Value.RecipesPerCuisine ?? 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AnsiConsole.Write(new FigletText(nameof(AIDataGenerator)).Color(Color.DodgerBlue1));

        var grid = new Grid();
        grid.AddColumns(2);
        grid.AddRow("Chat Completions",
            "[bold]:sparkles:Updated GPT-3.5 Turbo[/] [dim]gpt-3.5-turbo-1106[/] / [bold]:sparkles:GPT-4 Turbo[/] [dim]gpt-4-turbo-preview[/]");
        //grid.AddRow("Embeddings",
        //    "[bold]:sparkles:Ada V2[/] [dim]text-embedding-ada-002[/] / [bold]:sparkles:Embedding V3 small[/] [dim]text-embedding-3-small[/]");
        grid.AddRow("Image Generation",
            "[bold]:sparkles:DALL-E 2[/] [dim]dall-e-2[/] / [bold]:sparkles:DALL-E 3[/] [dim]dall-e-3[/]");

        var panel = new Panel(grid)
        {
            Header = new PanelHeader("[bold]Supported OpenAI Models[/]")
        };

        AnsiConsole.Write(panel);

        var recipeList = await GenerateRecipeListAsync(stoppingToken);

        await GenerateImagesAsync(recipeList, stoppingToken);

        var outputDirectory =
            Path.Combine(AppContext.BaseDirectory, "Data", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        Directory.CreateDirectory(outputDirectory);

        await ProcessImagesAsync(recipeList, outputDirectory, stoppingToken);

        var cuisines = recipeList.Cuisines?.Select(x => new Cuisine
        {
            Name = x.Name,
            Recipes = x.Recipes?.Select(y => new Recipe
            {
                Name = y.Name,
                CoverImage = y.CoverImage,
                CoverImagePrompt = y.CoverImagePrompt,
                Description = y.Description,
                Ingredients = [.. y.Ingredients],
                Instructions = new MarkdownData
                {
                    Markdown = y.InstructionsMarkdown
                }
            }).ToList()
        }).ToList();

        var json = JsonSerializer.Serialize(cuisines, s_jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Cuisines.json"), json, stoppingToken);

        applicationLifetime.StopApplication();
    }

    private async Task<GeneratedRecipeList> GenerateRecipeListAsync(CancellationToken cancellationToken)
    {
        GeneratedRecipeList? recipeList = null;

        var startTime = DateTime.UtcNow;

        await AnsiConsole.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new SpinnerColumn()
            ])
            .StartAsync(async ctx =>
            {
                var recipeListTask = ctx.AddTask("Recipe list");

                recipeList = await textGenerator.GenerateDataFromChatCompletions(
                    new GeneratedRecipeList
                    {
                        Cuisines =
                        [
                            new GeneratedCuisine
                            {
                                Name = "string",
                                Recipes = [new GeneratedRecipe { Name = "string" }]
                            }
                        ]
                    },
                    "You are a helpful assistant that creates recipe lists.",
                    $"Create a list of popular homemade recipes with {_recipesPerCuisine} recipes each from the following cuisines: {string.Join(", ", _cuisines)}",
                    cancellationToken);

                recipeListTask.Value = 100.0;
                recipeListTask.StopTask();

                var cuisines = recipeList?.Cuisines ?? Enumerable.Empty<GeneratedCuisine>();

                Dictionary<GeneratedCuisine, ProgressTask> progressTasks = [];

                foreach (var cuisine in cuisines)
                {
                    progressTasks[cuisine] = ctx.AddTask($"{cuisine.Name} recipes", false);
                }

                List<Task> tasks = [];

                await Parallel.ForEachAsync(cuisines,
                    new ParallelOptions
                    {
                        CancellationToken = cancellationToken,
                        MaxDegreeOfParallelism = _recipeGenerationMaxConcurrency
                    },
                    async (cuisine, cancellationToken) =>
                    {
                        var task = progressTasks[cuisine];
                        task.StartTask();

                        var increment = 1.0 / cuisine.Recipes!.Count * 100.0;

                        foreach (var recipe in cuisine.Recipes ?? Enumerable.Empty<GeneratedRecipe>())
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            try
                            {
                                var generatedRecipe = await textGenerator.GenerateDataFromChatCompletions(
                                    new GeneratedRecipe
                                    {
                                        CoverImagePrompt =
                                            "DALL-E prompt for generating image of plated finished recipe",
                                        Description = "string",
                                        Ingredients = ["string"],
                                        InstructionsMarkdown = "string"
                                    },
                                    "You are a helpful assistant that creates recipes.",
                                    $"Create a recipe for {recipe.Name}.",
                                    cancellationToken) ?? throw new NullReferenceException("Recipe data is null");

                                recipe.CoverImagePrompt = generatedRecipe.CoverImagePrompt;
                                recipe.Description = generatedRecipe.Description;
                                recipe.Ingredients = generatedRecipe.Ingredients;
                                recipe.InstructionsMarkdown = generatedRecipe.InstructionsMarkdown;
                            }
                            catch
                            {
                                logger.LogWarning("Unable to generate recipe for {Recipe}", recipe.Name);
                            }

                            task.Increment(increment);
                        }

                        task.Value = 100.0;
                        task.StopTask();
                    });
            });

        var recipes = recipeList?.Cuisines?.SelectMany(x => x.Recipes ?? Enumerable.Empty<GeneratedRecipe>());

        if (!(recipes?.Any() ?? false))
        {
            throw new Exception("Unable to generate recipe list");
        }

        var elapsed = DateTime.UtcNow - startTime;

        AnsiConsole.MarkupLine($":three_o_clock: Elapsed time: {elapsed}");
        AnsiConsole.MarkupLine($":clipboard: Recipes generated: {recipes.Count()}");

        return recipeList!;
    }

    private async Task GenerateImagesAsync(GeneratedRecipeList recipeList,
        CancellationToken cancellationToken)
    {
        var recipes = recipeList.Cuisines?.SelectMany(x => x.Recipes ?? Enumerable.Empty<GeneratedRecipe>());

        if (!(recipes?.Any() ?? false))
        {
            throw new ArgumentException("No recipes were provided", nameof(recipeList));
        }

        var startTime = DateTime.UtcNow;

        await AnsiConsole.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new SpinnerColumn()
            ])
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Recipe images");
                task.StartTask();

                var increment = 1.0 / recipes.Count() * 100.0;

                await Parallel.ForEachAsync(recipes,
                    new ParallelOptions
                    {
                        CancellationToken = cancellationToken,
                        MaxDegreeOfParallelism = _imageGenerationMaxConcurrency
                    },
                    async (recipe, cancellationToken) =>
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(recipe.CoverImagePrompt))
                            {
                                throw new NullReferenceException("Cover image prompt is null");
                            }

                            recipe.CoverImage =
                                await imageGenerator.GenerateImageAsync(recipe.CoverImagePrompt, cancellationToken);
                        }
                        catch
                        {
                            logger.LogWarning("Unable to generate image for {Recipe}", recipe.Name);
                        }

                        task.Increment(increment);
                    });

                task.Value = 100.0;
                task.StopTask();
            });

        var elapsed = DateTime.UtcNow - startTime;

        var imageCount = recipes.Where(x => !string.IsNullOrEmpty(x.CoverImage)).Count();

        AnsiConsole.MarkupLine($":three_o_clock: Elapsed time: {elapsed}");
        AnsiConsole.MarkupLine($":camera: Images generated: {imageCount}");
    }

    private async Task ProcessImagesAsync(GeneratedRecipeList recipeList, string directory,
        CancellationToken cancellationToken)
    {
        var recipes = recipeList.Cuisines?.SelectMany(x => x.Recipes ?? Enumerable.Empty<GeneratedRecipe>())
            .Where(x => !string.IsNullOrEmpty(x.CoverImage));

        if (!(recipes?.Any() ?? false))
        {
            throw new ArgumentException("No recipes with cover images were provided", nameof(recipeList));
        }

        using HttpClient client = new();

        var startTime = DateTime.UtcNow;

        await AnsiConsole.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new SpinnerColumn()
            ])
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("Image processing");
                task.StartTask();

                var increment = 1.0 / recipes.Count() * 100.0;

                foreach (var recipe in recipes)
                {
                    var canonicalName =
                        new string(recipe.Name!.Normalize()
                                .Where(x => char.IsAsciiLetterOrDigit(x) || char.IsWhiteSpace(x)).ToArray()).ToLower()
                            .Replace(' ', '-');

                    try
                    {
                        var bytes = await client.GetByteArrayAsync(recipe.CoverImage, cancellationToken);

                        using var bitmap = SKBitmap.Decode(bytes);
                        using var data = bitmap.Encode(SKEncodedImageFormat.Webp, _imageQuality);

                        await File.WriteAllBytesAsync(
                            Path.Combine(directory, $"{canonicalName}.webp"),
                            data.AsSpan().ToArray(),
                            cancellationToken);

                        recipe.CoverImage = $"{canonicalName}.webp";
                    }
                    catch
                    {
                        logger.LogWarning("Failed to download image for {Recipe}", recipe.Name);
                    }

                    task.Increment(increment);
                }

                task.Value = 100.0;
                task.StopTask();
            });

        var elapsed = DateTime.UtcNow - startTime;

        AnsiConsole.MarkupLine($":three_o_clock: Elapsed time: {elapsed}");
        AnsiConsole.MarkupLine(
            $":camera: Images processed: {Directory.GetFiles(directory).Where(x => x.EndsWith(".webp")).Count()}");
    }
}
