using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeBook.DataGenerator;
using RecipeBook.DataGenerator.Services;

Console.OutputEncoding = Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTextGenerationService(builder.Configuration.GetSection("TextGenerationService"));
builder.Services.AddImageGenerationService(builder.Configuration.GetSection("ImageGenerationService"));

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.Run();
