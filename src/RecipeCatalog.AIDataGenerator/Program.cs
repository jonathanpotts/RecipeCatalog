using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.TextToImage;
using RecipeCatalog.AIDataGenerator;
using RecipeCatalog.AIDataGenerator.Options;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<ChatCompletion>()
    .BindConfiguration("OpenAI:ChatCompletion")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IChatCompletionService>(services =>
{
    var options = services.GetRequiredService<IOptions<ChatCompletion>>().Value;

    if (string.IsNullOrEmpty(options.Endpoint))
    {
        return new OpenAIChatCompletionService(options.Model, options.Key);
    }
    else
    {
        return new AzureOpenAIChatCompletionService(options.Model, options.Endpoint, options.Key);
    }
});

builder.Services.AddOptions<TextEmbedding>()
    .BindConfiguration("OpenAI:TextEmbedding")
    .ValidateDataAnnotations()
    .ValidateOnStart();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddSingleton<ITextEmbeddingGenerationService>(services =>
{
    var options = services.GetRequiredService<IOptions<TextEmbedding>>().Value;

    if (string.IsNullOrEmpty(options.Endpoint))
    {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return new OpenAITextEmbeddingGenerationService(options.Model, options.Key);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
    else
    {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return new AzureOpenAITextEmbeddingGenerationService(options.Model, options.Endpoint, options.Key);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
});
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Services.AddOptions<TextToImage>()
    .BindConfiguration("OpenAI:TextToImage")
    .ValidateDataAnnotations()
    .ValidateOnStart();

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddSingleton<ITextToImageService>(services =>
{
    var options = services.GetRequiredService<IOptions<TextToImage>>().Value;

    if (string.IsNullOrEmpty(options.Endpoint))
    {
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return new OpenAITextToImageService(options.Key);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
    else
    {
        if (string.IsNullOrEmpty(options.Deployment))
        {
            throw new Exception($"The {nameof(TextToImage)} {nameof(options.Deployment)} must be provided when an {nameof(options.Endpoint)} is provided.");
        }

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return new AzureOpenAITextToImageService(options.Deployment, options.Endpoint, options.Key, null);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
});
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

builder.Services.AddWorker(builder.Configuration.GetSection("Worker"));

var app = builder.Build();

try
{
    app.Run();
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
    Environment.Exit(-1);
}
