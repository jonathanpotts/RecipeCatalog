using System.Text.Json;
using System.Text.Json.Serialization;
using JonathanPotts.RecipeCatalog.AIDataGenerator.Models;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.Domain.Shared.ValueObjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextToImage;
using SkiaSharp;
using Spectre.Console;

namespace JonathanPotts.RecipeCatalog.AIDataGenerator;

internal class Worker(
    IOptions<Options.Worker> options,
    IHostApplicationLifetime applicationLifetime,
    ILogger<Worker> logger,
    IChatCompletionService chatCompletionService,
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    ITextEmbeddingGenerationService textEmbeddingGenerationService,
    ITextToImageService textToImageService) : BackgroundService
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IReadOnlyList<string> _cuisines = options.Value.Cuisines!.AsReadOnly();
    private readonly int _imageGenerationMaxConcurrency = options.Value.ImageGenerationMaxConcurrency ?? 1;
    private readonly int _imageQuality = options.Value.ImageQuality ?? 60;
    private readonly int _recipeGenerationMaxConcurrency = options.Value.RecipeGenerationMaxConcurrency ?? 5;
    private readonly int _recipesPerCuisine = options.Value.RecipesPerCuisine ?? 1;
    private const int _imageWidth = 1024;
    private const int _imageHeight = 1024;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AnsiConsole.Write(new FigletText(nameof(AIDataGenerator)).Color(Color.DodgerBlue1));

        var openAIGrid = new Grid();
        openAIGrid.AddColumns(2);
        openAIGrid.AddRow("Chat Completions",
            "[bold]:sparkles:GPT-3.5 Turbo[/] [dim]gpt-3.5-turbo[/] / [bold]:sparkles:GPT-4o[/] [dim]gpt-4o[/]");
        openAIGrid.AddRow("Embeddings",
            "[bold]:sparkles:Ada V2[/] [dim]text-embedding-ada-002[/] / [bold]:sparkles:Embedding V3 small[/] [dim]text-embedding-3-small[/]");
        openAIGrid.AddRow("Image Generation",
            "[bold]:sparkles:DALL-E 3[/] [dim]dall-e-3[/]");

        var openAIPanel = new Panel(openAIGrid)
        {
            Header = new PanelHeader("[bold]Supported OpenAI Models[/]")
        };

        AnsiConsole.Write(openAIPanel);

        var azureOpenAIGrid = new Grid();
        azureOpenAIGrid.AddColumns(2);
        azureOpenAIGrid.AddRow("Chat Completions",
            "[bold]:sparkles:GPT-3.5 Turbo[/] [dim]gpt-35-turbo (0125)[/] / [bold]:sparkles:GPT-4o[/] [dim]gpt-4o (2024-05-13)[/]");
        azureOpenAIGrid.AddRow("Embeddings",
            "[bold]:sparkles:Ada V2[/] [dim]text-embedding-ada-002[/] / [bold]:sparkles:Embedding V3 small[/] [dim]text-embedding-3-small[/]");
        azureOpenAIGrid.AddRow("Image Generation",
            "[bold]:sparkles:DALL-E 3[/] [dim]dall-e-3[/]");

        var azureOpenAIPanel = new Panel(azureOpenAIGrid)
        {
            Header = new PanelHeader("[bold]Supported Azure OpenAI Service Models[/]")
        };

        AnsiConsole.Write(azureOpenAIPanel);

        var recipeList = await GenerateRecipeListAsync(stoppingToken);

        var outputDirectory =
            Path.Combine(AppContext.BaseDirectory, "Data", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        Directory.CreateDirectory(outputDirectory);

        await GenerateImagesAsync(recipeList, outputDirectory, stoppingToken);
        await GenerateEmbeddingsAsync(recipeList, stoppingToken);

        var cuisines = recipeList.Cuisines?.Select(x => new Cuisine
        {
            Name = x.Name,
            Recipes = x.Recipes?.Select(y => new Recipe
            {
                Name = y.Name,
                NameEmbeddings = y.NameEmbeddings,
                CoverImage = new ImageData
                {
                    Url = y.CoverImage,
                    AltText = y.CoverImagePrompt
                },
                Description = y.Description,
                DescriptionEmbeddings = y.DescriptionEmbeddings,
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

                recipeList = await chatCompletionService.GenerateDataFromChatCompletionsAsync(
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
                                var generatedRecipe = await chatCompletionService.GenerateDataFromChatCompletionsAsync(
                                    new GeneratedRecipe
                                    {
                                        CoverImagePrompt =
                                            "description of a photorealistic image of the plated recipe",
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
        AnsiConsole.WriteLine();

        return recipeList!;
    }

    private async Task GenerateImagesAsync(GeneratedRecipeList recipeList, string directory,
        CancellationToken cancellationToken)
    {
        var recipes = recipeList.Cuisines?.SelectMany(x => x.Recipes ?? Enumerable.Empty<GeneratedRecipe>());

        if (!(recipes?.Any() ?? false))
        {
            throw new ArgumentException("No recipes were provided", nameof(recipeList));
        }

        var imagesDirectory = Path.Combine(directory, "Images");
        Directory.CreateDirectory(imagesDirectory);

        using HttpClient client = new();

        var startTime = DateTime.UtcNow;

        await AnsiConsole.Status()
            .StartAsync("Generating images", async ctx =>
            {
                await Parallel.ForEachAsync(recipes,
                    new ParallelOptions
                    {
                        CancellationToken = cancellationToken,
                        MaxDegreeOfParallelism = _imageGenerationMaxConcurrency
                    },
                    async (recipe, cancellationToken) =>
                    {
                        AnsiConsole.WriteLine($"Generating image for {recipe.Name}...");

                        if (string.IsNullOrEmpty(recipe.CoverImagePrompt))
                        {
                            throw new NullReferenceException("Cover image prompt is null");
                        }

                        var canonicalName =
                            new string(recipe.Name!.Normalize()
                                    .Where(x => char.IsAsciiLetterOrDigit(x) || char.IsWhiteSpace(x)).ToArray())
                                .ToLower()
                                .Replace(' ', '-');

                        try
                        {
#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                            var generatedImage = await textToImageService.GenerateImageAsync(recipe.CoverImagePrompt, _imageWidth, _imageHeight, cancellationToken: cancellationToken);
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

                            var bytes = await client.GetByteArrayAsync(generatedImage, cancellationToken);

                            using var bitmap = SKBitmap.Decode(bytes);
                            using var data = bitmap.Encode(SKEncodedImageFormat.Webp, _imageQuality);

                            await File.WriteAllBytesAsync(
                                Path.Combine(imagesDirectory, $"{canonicalName}.webp"),
                                data.AsSpan().ToArray(),
                                cancellationToken);

                            recipe.CoverImage = $"{canonicalName}.webp";
                        }
                        catch
                        {
                            logger.LogWarning("Unable to generate image for {Recipe}", recipe.Name);
                        }
                    });
            });

        var elapsed = DateTime.UtcNow - startTime;

        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($":three_o_clock: Elapsed time: {elapsed}");
        AnsiConsole.MarkupLine(
            $":camera: Images generated: {Directory.GetFiles(directory).Where(x => x.EndsWith(".webp")).Count()}");
        AnsiConsole.WriteLine();
    }

    private async Task GenerateEmbeddingsAsync(GeneratedRecipeList recipeList, CancellationToken cancellationToken)
    {
        var recipes = recipeList.Cuisines?.SelectMany(x => x.Recipes ?? Enumerable.Empty<GeneratedRecipe>());

        if (!(recipes?.Any() ?? false))
        {
            throw new ArgumentException("No recipes were provided", nameof(recipeList));
        }

        var startTime = DateTime.UtcNow;

        await AnsiConsole.Status()
            .StartAsync("Generating embeddings", async ctx =>
            {
                await Parallel.ForEachAsync(recipes,
                    new ParallelOptions
                    {
                        CancellationToken = cancellationToken,
                        MaxDegreeOfParallelism = _recipeGenerationMaxConcurrency
                    },
                    async (recipe, cancellationToken) =>
                    {
                        AnsiConsole.WriteLine($"Generating embeddings for {recipe.Name}...");

                        var name = recipe.Name?.Trim().ReplaceLineEndings().Replace(Environment.NewLine, " ").ToLower();

                        var description = recipe.Description?.Trim().ReplaceLineEndings().Replace(Environment.NewLine, " ").ToLower();

                        try
                        {
                            if (!string.IsNullOrEmpty(name))
                            {
                                var nameEmbeddings =
                                    (await textEmbeddingGenerationService.GenerateEmbeddingsAsync([name], cancellationToken: cancellationToken)).First();

                                recipe.NameEmbeddings = nameEmbeddings.ToArray();
                            }

                            if (!string.IsNullOrEmpty(description))
                            {
                                var descriptionEmbeddings =
                                    (await textEmbeddingGenerationService.GenerateEmbeddingsAsync([description], cancellationToken: cancellationToken)).First();

                                recipe.DescriptionEmbeddings = descriptionEmbeddings.ToArray();
                            }
                        }
                        catch
                        {
                            logger.LogWarning("Unable to generate embeddings for {Recipe}", recipe.Name);
                        }
                    });
            });

        var elapsed = DateTime.UtcNow - startTime;

        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($":three_o_clock: Elapsed time: {elapsed}");
        AnsiConsole.MarkupLine(
            $":input_numbers: Embeddings generated: {recipes.Count(x => x.NameEmbeddings != null)}");
        AnsiConsole.WriteLine();
    }
}
