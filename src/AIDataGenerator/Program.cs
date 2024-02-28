using System.Text;
using JonathanPotts.RecipeCatalog.AI;
using JonathanPotts.RecipeCatalog.AIDataGenerator;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

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
