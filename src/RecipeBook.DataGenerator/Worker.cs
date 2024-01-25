using System.Text.Json;
using IdGen;
using Markdig;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecipeBook.Api.Models;
using RecipeBook.DataGenerator.Models;
using RecipeBook.DataGenerator.Services;
using SkiaSharp;
using Spectre.Console;

namespace RecipeBook.DataGenerator;

internal class Worker(
    IOptions<WorkerOptions> options,
    IHostApplicationLifetime applicationLifetime,
    ILogger<Worker> logger,
    ITextGenerator textGenerator,
    IImageGenerator imageGenerator,
    IdGenerator idGenerator) : BackgroundService
{
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    private static readonly MarkdownPipeline s_pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()
        .UseReferralLinks(["ugc"])
        .Build();

    private readonly IReadOnlyList<string> _cuisines = options.Value.Cuisines!.AsReadOnly();
    private readonly int _imageGenerationMaxConcurrency = options.Value.ImageGenerationMaxConcurrency ?? 2;
    private readonly int _imageQuality = options.Value.ImageQuality ?? 60;
    private readonly int _recipeGenerationMaxConcurrency = options.Value.RecipeGenerationMaxConcurrency ?? 5;
    private readonly int _recipesPerCuisine = options.Value.RecipesPerCuisine ?? 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AnsiConsole.Write(new FigletText("Data Generator").Color(Color.DodgerBlue1));

        var grid = new Grid();
        grid.AddColumns(3);
        grid.AddRow("Chat Completions",
            "[bold]:sparkles:GPT-3.5 Turbo (1106)[/] / [bold]:sparkles:GPT-4 Turbo (1106)[/]",
            "[dim]gpt-3.5-turbo-1106 / gpt-4-1106-preview[/]");
        grid.AddRow("Text Embeddings", "[bold]:sparkles:Ada (version 2)[/]", "[dim]text-embedding-ada-002[/]");
        grid.AddRow("Image Generation", "[bold]:sparkles:DALL-E 2[/] / [bold]:sparkles:DALL-E 3[/]",
            "[dim]dall-e-2 / dall-e-3[/]");

        var panel = new Panel(grid)
        {
            Header = new PanelHeader("[bold]OpenAI Models Used[/]")
        };

        AnsiConsole.Write(panel);

        var recipes = await GenerateRecipesAsync(stoppingToken);

        var imageUrls = await GenerateImagesAsync(recipes, stoppingToken);

        var outputDirectory =
            Path.Combine(AppContext.BaseDirectory, "Data", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        Directory.CreateDirectory(outputDirectory);

        var imagesDirectory = Path.Combine(outputDirectory, "Images");
        Directory.CreateDirectory(imagesDirectory);

        await ProcessImagesAsync(imageUrls, imagesDirectory, stoppingToken);

        var json = JsonSerializer.Serialize(recipes, jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, "Cuisines.json"), json, stoppingToken);

        applicationLifetime.StopApplication();
    }

    private async Task<List<Cuisine>> GenerateRecipesAsync(CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        List<Cuisine> cuisineList = [];

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

                var recipeList = await textGenerator.GenerateDataFromChatCompletions(
                    new RecipeList
                    {
                        Cuisines =
                        [
                            new CuisineData
                            {
                                Name = "string",
                                Recipes = ["string"]
                            }
                        ]
                    },
                    "You are a helpful assistant that creates recipe lists.",
                    $"Create a list of homemade recipes popular in the United States with {_recipesPerCuisine} recipes each from the following cuisines: {string.Join(", ", _cuisines)}",
                    cancellationToken);

                recipeListTask.Value = 100.0;
                recipeListTask.StopTask();

                var cuisines = recipeList?.Cuisines ?? Enumerable.Empty<CuisineData>();

                Dictionary<CuisineData, ProgressTask> progressTasks = [];

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

                        Cuisine newCuisine = new()
                        {
                            Name = cuisine.Name,
                            Recipes = []
                        };

                        foreach (var name in cuisine.Recipes ?? Enumerable.Empty<string>())
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            try
                            {
                                var recipe = await textGenerator.GenerateDataFromChatCompletions(
                                    new RecipeData
                                    {
                                        CoverImagePrompt =
                                            "descriptive DALL-E prompt for generating image of plated finished recipe",
                                        Description = "string",
                                        Ingredients = ["string"],
                                        InstructionsMarkdown = "string"
                                    },
                                    "You are a helpful assistant that creates recipes.",
                                    $"Create a recipe for {name}.",
                                    cancellationToken) ?? throw new NullReferenceException("Recipe data is null");

                                newCuisine.Recipes.Add(new Recipe
                                {
                                    Id = idGenerator.CreateId(),
                                    Name = name,
                                    CoverImagePrompt = recipe.CoverImagePrompt,
                                    Description = recipe.Description,
                                    Created = DateTime.UtcNow,
                                    Ingredients = recipe.Ingredients?.ToArray(),
                                    Instructions = new MarkdownData
                                    {
                                        Markdown = recipe.InstructionsMarkdown,
                                        Html = recipe.InstructionsMarkdown == null
                                            ? null
                                            : Markdown.ToHtml(recipe.InstructionsMarkdown, s_pipeline)
                                    }
                                });
                            }
                            catch
                            {
                                logger.LogWarning("Unable to generate recipe for {Recipe}", name);
                            }

                            task.Increment(increment);
                        }

                        cuisineList.Add(newCuisine);

                        task.Value = 100.0;
                        task.StopTask();
                    });
            });

        var elapsed = DateTime.UtcNow - startTime;

        AnsiConsole.MarkupLine($":three_o_clock: Elapsed time: {elapsed}");
        AnsiConsole.MarkupLine($":clipboard: Recipes generated: {cuisineList.SelectMany(x => x.Recipes!).Count()}");

        return cuisineList;
    }

    private async Task<Dictionary<Recipe, string>> GenerateImagesAsync(List<Cuisine> cuisines,
        CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        Dictionary<Recipe, string> urls = [];

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

                var recipes = cuisines.SelectMany(x => x.Recipes ?? Enumerable.Empty<Recipe>());
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

                            urls.Add(recipe,
                                await imageGenerator.GenerateImageAsync(recipe.CoverImagePrompt, cancellationToken));
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

        AnsiConsole.MarkupLine($":three_o_clock: Elapsed time: {elapsed}");
        AnsiConsole.MarkupLine($":camera: Images generated: {urls.Count}");

        return urls;
    }

    private async Task ProcessImagesAsync(Dictionary<Recipe, string> imageUrls, string directory,
        CancellationToken cancellationToken)
    {
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

                var increment = 1.0 / imageUrls.Count * 100.0;

                foreach (var (recipe, url) in imageUrls)
                {
                    try
                    {
                        var bytes = await client.GetByteArrayAsync(url, cancellationToken);

                        using var bitmap = SKBitmap.Decode(bytes);
                        using var data = bitmap.Encode(SKEncodedImageFormat.Webp, _imageQuality);

                        await File.WriteAllBytesAsync(
                            Path.Combine(directory, $"{recipe.Id}.webp"),
                            data.AsSpan().ToArray(),
                            cancellationToken);

                        recipe.CoverImage = $"{recipe.Id}.webp";
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
