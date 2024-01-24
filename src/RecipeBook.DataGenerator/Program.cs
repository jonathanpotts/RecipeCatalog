using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeBook.DataGenerator;
using RecipeBook.DataGenerator.Services;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTextGenerator(builder.Configuration.GetSection("TextGenerator"));
builder.Services.AddImageGenerator(builder.Configuration.GetSection("ImageGenerator"));

builder.Services.AddHostedService<Worker>();

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
