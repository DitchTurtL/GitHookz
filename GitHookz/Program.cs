using Blazored.SessionStorage;
using Discord.Interactions;
using Discord.WebSocket;
using GitHookz.Components;
using GitHookz.Data.Bot;
using GitHookz.Data.State;
using GitHookz.Services;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Any, 5000); // HTTP
    options.Listen(System.Net.IPAddress.Any, 5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});


// Load environment variables from .env file
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", ".env");
if (!File.Exists(envFile))
    Log.Logger.Information("No .env file found. Skipping environment variable loading.");
else
    DotNetEnv.Env.Load(envFile);

// Add platform services to the container.
builder.Services
    .AddMudServices()
    .AddBlazoredSessionStorage()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Add application services to the container.
builder.Services
    .AddHostedService<HostedBotService>()
    .AddScoped<NavMenuState>()
    .AddSingleton<IBotService, BotService>()
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton<InteractionHandler>()
    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), new InteractionServiceConfig()))
    .AddSingleton<IDatabaseService, DatabaseService>()
    .AddSingleton<IGithubService, GithubService>()
    .AddSingleton<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/webhook", async ([FromBody] JsonDocument payload, HttpContext context, IGithubService githubService) =>
{
    await githubService.HandleWebhook(payload, context);
    return Results.Ok();
});

app.Run();
