using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Globalization;
using System.Reflection;
using GitHookz.Data.Bot;
using System.Reflection.Metadata;

namespace GitHookz.Services;

public class BotService : IBotService
{
    private readonly ILogger<BotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly InteractionHandler _interactionHandler;
    private readonly InteractionService _interactionService;

    public BotService(ILogger<BotService> logger, DiscordSocketClient client, InteractionHandler interactionHandler, InteractionService interactionService)
    {
        _logger = logger;

        _client = client;
        _client.Log += LogAsync;

        _interactionHandler = interactionHandler;
        _interactionService = interactionService;

    }

    private static readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
        AlwaysDownloadUsers = true,
    };

    public async Task StartBotAsync()
    {
        _logger.LogInformation("Bot Service is starting.");

        // Get bot token
        var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
        if (string.IsNullOrEmpty(botToken))
        {
            _logger.LogError("Bot token is not set.");
            return;
        }

        // Initialize interaction handler
        await _interactionHandler.InitializeAsync();

        await _client.LoginAsync(TokenType.Bot, botToken);
        await _client.StartAsync();
        _client.MessageReceived += MessageReceivedAsync;
    }

    private Task MessageReceivedAsync(SocketMessage message)
    {
        _logger.LogInformation("Message received: {message}", message.Content);
        return Task.CompletedTask;
    }

    private Task LogAsync(LogMessage message)
    {
        _logger.LogInformation(message.ToString());
        return Task.CompletedTask;
    }
}
