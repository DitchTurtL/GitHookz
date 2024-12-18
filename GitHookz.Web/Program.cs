using Discord.WebSocket;
using GitHookz.Core.Data;
using GitHookz.Core.Services;
using GitHookz.Web.Components;
using MudBlazor.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services
    .Configure<GitHookzAppSettings>(builder.Configuration.GetSection("GitHookz"))
    .AddSingleton<IBotService, BotService>()
    .AddSingleton<IDatabaseService, DatabaseService>()
    .AddSingleton<InteractionHandlerService>()
    .AddSingleton<DiscordSocketClient>()
    .AddHostedService<HostedBotService>()
    .AddMudServices()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
