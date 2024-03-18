using GitHookz.Data;
using Discord.Interactions;
using Discord.WebSocket;
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
    .AddHostedService<HostedClientService>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


var webhookEndpoint = _configuration["webhook_endpoint"] ?? StringConstants.DEFAULT_WEBHOOK_ENDPOINT;
app.MapGitHubWebhooks(webhookEndpoint);
var externalUrl = _configuration["external_url"] ?? StringConstants.DEFAULT_EXTERNAL_URL;

var urls = _configuration["AllowedHosts"] ?? "*";
app.Urls.Add(urls);
Log.Information($"Listening on {externalUrl}{webhookEndpoint}");
Log.Information($"Hosting on {urls}");
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
