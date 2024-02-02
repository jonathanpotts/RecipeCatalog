using FluentValidation;
using IdGen;
using IdGen.DependencyInjection;
using JonathanPotts.RecipeCatalog.WebApi.Apis;
using JonathanPotts.RecipeCatalog.WebApi.Authorization;
using JonathanPotts.RecipeCatalog.WebApi.Data;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, $"{nameof(ApplicationDbContext)}.db")}"));

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthorizationHandler, CuisineAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RecipeAuthorizationHandler>();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<DbMigrator>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationRulesToSwagger();

// Add IdGen for creating Snowflake IDs
var generatorId = builder.Configuration.GetValue("GeneratorId", 0);
var epoch = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
builder.Services.AddIdGen(generatorId, () => new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/api/v1/identity").WithTags("Identity").MapIdentityApi<IdentityUser>();

app.MapCuisinesApi();
app.MapRecipesApi();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DbMigrator>().Migrate();
}

app.Run();
