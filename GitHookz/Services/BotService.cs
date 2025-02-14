using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using GitHookz.Data.Bot;
using GitHookz.Data.Bot.ResponseArguments;

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

    private IMessageChannel? GetChannelById(string channelId)
    {
        var channel = _client.GetChannel(ulong.Parse(channelId)) as IMessageChannel;
        if (channel == null)
        {
            _logger.LogError("Channel not found: {channelId}", channelId);
            return null;
        }
        return channel;
    }

    public async Task SendPingMessage(string channelId, PingReponseArguments arguments)
    {
        var channel = GetChannelById(channelId);
        if (channel == null) return;

        var message = $"Your {arguments.RepositoryDetails.RepositoryName} repository is connected to GitHookz!";
        var embed = new EmbedBuilder()
            .WithTitle("GitHookz is working!")
            .WithDescription(message)
            .WithColor(Color.Blue)
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await channel.SendMessageAsync(embed: embed);
    }

    public async Task SendPushMessage(string channelId, PushResponseArguments arguments)
    {
        var channel = GetChannelById(channelId);
        if (channel == null) return;

        var message = $"""
            {arguments.SenderDetails.Username} pushed new code to {arguments.RepositoryDetails.RepositoryName}
            +{arguments.AddedCount}|{arguments.ModifiedCount}|-{arguments.RemovedCount} {arguments.CommitMessage}
            """; 

        var embed = new EmbedBuilder()
            .WithTitle("New Push")
            .WithThumbnailUrl(arguments.SenderDetails.AvatarUrl)
            .WithDescription(message)
            .WithColor(Color.Green)
            .WithTimestamp(DateTimeOffset.Now)
            .Build();

        await channel.SendMessageAsync(embed: embed);
    }

    public Task SendPullRequestMessage(string channelId, PullRequestResponseArguments arguments)
    {
        throw new NotImplementedException();
    }

    public Task SendPullRequestReviewCommentMessage(string channelId, PullRequestReviewCommentResponseArguments arguments)
    {
        throw new NotImplementedException();
    }

    public Task SendIssueMessage(string channelId, IssueResponseArguments arguments)
    {
        throw new NotImplementedException();
    }

    public Task SendIssueCommentMessage(string channelId, IssueCommentResponseArguments arguments)
    {
        throw new NotImplementedException();
    }

    public Task SendReleaseMessage(string channelId, ReleaseResponseArguments arguments)
    {
        throw new NotImplementedException();
    }

    public Task SendForkMessage(string channelId, ForkResponseArguments arguments)
    {
        throw new NotImplementedException();
    }

    public Task SendStarMessage(string channelId, StarResponseArguments arguments)
    {
        throw new NotImplementedException();
    }
}
