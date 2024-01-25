using System.Text;
using IdGen;
using IdGen.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RecipeBook.DataGenerator;
using RecipeBook.DataGenerator.Services;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

// Add IdGen for creating Snowflake IDs
var generatorId = builder.Configuration.GetValue("GeneratorId", 0);
var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
builder.Services.AddIdGen(generatorId, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

if (builder.Configuration.GetValue<bool>("TextGenerator:UseAzureOpenAI"))
{
    builder.Services.AddAzureOpenAITextGenerator(builder.Configuration.GetSection("TextGenerator"));
}
else
{
    builder.Services.AddOpenAITextGenerator(builder.Configuration.GetSection("TextGenerator"));
}

if (builder.Configuration.GetValue<bool>("ImageGenerator:UseAzureOpenAI"))
{
    if (builder.Configuration.GetValue<bool>("ImageGenerator:UseDallE3"))
    {
        builder.Services.AddAzureOpenAIImageGenerator(builder.Configuration.GetSection("ImageGenerator"));
    }
    else
    {
        builder.Services.AddAzureOpenAIDallE2ImageGenerator(builder.Configuration.GetSection("ImageGenerator"));
    }
}
else
{
    builder.Services.AddOpenAIImageGenerator(builder.Configuration.GetSection("ImageGenerator"));
}

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
