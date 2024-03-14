using Discord.Interactions;
using Discord.WebSocket;
using GitHookz.Services;
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
    .AddHostedService<HostedClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.MapPost("/webhook", async context =>
{
    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
    {
        string body = await reader.ReadToEndAsync();
        Log.Information(body);
        Console.WriteLine(body);

        // Parse the JSON body
        //JObject payload = JObject.Parse(body);

        // Handle the GitHub webhook payload
        //Console.WriteLine(payload.ToString());
    }
    context.Response.StatusCode = StatusCodes.Status200OK;
});


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
});

app.Urls.Add("http://192.168.0.36:5048/");
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
