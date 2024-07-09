using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using DotaWebStats.Components;
using DotaWebStats.Components.Pages;
using Services; // Add the namespace for your services

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISteamDataService, SteamDataService>(); // Register the SteamDataService

// Register the SteamAuthService
builder.Services.AddScoped<SteamAuthService>();
builder.Services.AddScoped<ISteamAuthService, SteamAuthService>();
builder.Services.AddScoped<IDotaDataService, DotaDataService>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();