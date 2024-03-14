using Discord.WebSocket;
using Discord;
using Serilog;

namespace GitHookz.Services;

public class HostedClientService : IHostedService
{
    private readonly IInteractionHandler _interactionHandler;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly string _token;

    public HostedClientService(IConfiguration configuration, IInteractionHandler interactionHandler, DiscordSocketClient client)
    {
        _configuration = configuration;
        _token = _configuration["discord_token"] ?? "Missing Token";
        _interactionHandler = interactionHandler;
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Log += LogAsync;

        await _interactionHandler.InitializeAsync();

        Log.Information($"Logging in: {_token}");
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        Log.Information(log.Message);
        if (log.Exception != null)
            Log.Warning(log.Exception.ToString());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

}
