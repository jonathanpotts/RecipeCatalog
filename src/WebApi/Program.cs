using JonathanPotts.RecipeCatalog.Application;
using JonathanPotts.RecipeCatalog.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var generatorId = builder.Configuration.GetValue("GeneratorId", 0);
builder.Services.AddWebApiServices(generatorId);
builder.Services.AddDbMigrator();

builder.Services.AddIdentity();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDbMigrator();

app.MapWebApi();

app.Run();
