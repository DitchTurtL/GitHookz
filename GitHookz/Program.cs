using Discord.Interactions;
using Discord.WebSocket;
using GitHookz.Services;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;
using Serilog;
using Serilog.Events;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Configure logger
var _logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information)
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Build configuration
var _configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container
builder.Services
    .AddSingleton<Serilog.ILogger>(_logger)
    .AddSingleton(_configuration)
    .AddSingleton(new DiscordSocketConfig()
    {
        GatewayIntents = Discord.GatewayIntents.AllUnprivileged | Discord.GatewayIntents.GuildMembers,
        AlwaysDownloadUsers = true
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
    .AddSingleton<InteractionHandler>()
    .AddSingleton<WebhookEventProcessor, EventProcessor>()
    .AddHostedService<HostedClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.MapGitHubWebhooks("/webhooks");

app.Urls.Add("http://192.168.0.36:5048/");
app.Run();
