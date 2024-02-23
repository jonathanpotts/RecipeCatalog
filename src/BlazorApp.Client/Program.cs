using JonathanPotts.RecipeCatalog.Application.Contracts.Services;
using JonathanPotts.RecipeCatalog.BlazorApp.Client;
using JonathanPotts.RecipeCatalog.BlazorApp.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddScoped<ICuisineService, CuisineService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();

await builder.Build().RunAsync();
