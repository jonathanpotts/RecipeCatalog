using System.Text;
using IdGen;
using IdGen.DependencyInjection;
using JonathanPotts.RecipeCatalog.AIDataGenerator;
using JonathanPotts.RecipeCatalog.AIDataGenerator.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

// Add IdGen for creating Snowflake IDs
var generatorId = builder.Configuration.GetValue("GeneratorId", 0);
var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
builder.Services.AddIdGen(generatorId, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

builder.Services.AddAITextGenerator(builder.Configuration.GetSection("AITextGenerator"));
builder.Services.AddAIImageGenerator(builder.Configuration.GetSection("AIImageGenerator"));

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
