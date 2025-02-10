using Blazored.SessionStorage;
using Discord.Interactions;
using Discord.WebSocket;
using GitHookz.Components;
using GitHookz.Data.Bot;
using GitHookz.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

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

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
