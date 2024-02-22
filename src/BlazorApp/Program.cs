using JonathanPotts.RecipeCatalog.Application;
using JonathanPotts.RecipeCatalog.BlazorApp.Components;
using JonathanPotts.RecipeCatalog.BlazorApp.Components.Account;
using JonathanPotts.RecipeCatalog.Domain.Entities;
using JonathanPotts.RecipeCatalog.WebApi;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

var generatorId = builder.Configuration.GetValue("GeneratorId", 0);
builder.Services.AddWebApiServices(generatorId);
builder.Services.AddDbMigrator();

builder.Services.AddIdentityBlazor();

builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(JonathanPotts.RecipeCatalog.BlazorApp.Client.Components._Imports).Assembly);

app.UseDbMigrator();

app.MapWebApi();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
