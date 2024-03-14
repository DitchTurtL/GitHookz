using Discord.WebSocket;
using Discord;

namespace GitHookz.Services;

public class HostedClientService : IHostedService
{
    private readonly InteractionHandler _interactionHandler;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HostedClientService> _logger;

    public HostedClientService(ILogger<HostedClientService> logger, IConfiguration configuration, InteractionHandler interactionHandler, DiscordSocketClient client)
    {
        _logger = logger;
        _configuration = configuration;
        _interactionHandler = interactionHandler;
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += LogAsync;

        await _interactionHandler.InitializeAsync();

        await _client.LoginAsync(TokenType.Bot, _configuration["discord_token"]);
        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.LogInformation(log.Message);
        _logger.LogWarning(log.Exception?.ToString());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

}
