using Discord.Interactions;
using Discord.WebSocket;
using GitHookz.Data;
using GitHookz.Services;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File($"{StringConstants.LOG_DIR}/{StringConstants.LOG_FILE}", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Log.Information("Loading Configuration");
var _configuration = new ConfigurationBuilder()
    .AddJsonFile("Data/appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

Log.Information("Initializing Services");
builder.Services
    .AddSingleton<IConfiguration>(_configuration)
    .AddSingleton(new DiscordSocketConfig()
    {
        GatewayIntents = Discord.GatewayIntents.AllUnprivileged | Discord.GatewayIntents.GuildMembers,
        AlwaysDownloadUsers = true
    })
    .AddSingleton<DiscordSocketClient>()
    .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
    .AddSingleton<IInteractionHandler, InteractionHandler>()
    .AddSingleton<WebhookEventProcessor, EventProcessor>()
    .AddSingleton<IDatabaseService, DatabaseService>()
    .AddHostedService<HostedClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

var webhookEndpoint = _configuration["webhook_endpoint"] ?? StringConstants.DEFAULT_WEBHOOK_ENDPOINT;
app.MapGitHubWebhooks(webhookEndpoint);
var externalUrl = _configuration["external_url"] ?? StringConstants.DEFAULT_EXTERNAL_URL;

var urls = _configuration["hosting_url"] ?? "*";
app.Urls.Add(urls);
Log.Information($"Listening on {externalUrl}{webhookEndpoint}");
Log.Information($"Hosting on {urls}");
app.Run();