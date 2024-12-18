using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Options;
using GitHookz.Core.Data;
using Serilog;

namespace GitHookz.Core.Services;

public class BotService : IBotService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly GitHookzAppSettings _settings;

    private static readonly DiscordSocketConfig _socketConfig = new()
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
        AlwaysDownloadUsers = true,
    };

    public BotService(IOptions<GitHookzAppSettings> settings, DiscordSocketClient client, InteractionService handler)
    {
        _settings = settings.Value;
        _client = client;
        _handler = handler;
    }



    public async Task StartAsync()
    {
        _client.Log += LogAsync;
        await _client.LoginAsync(TokenType.Bot, _settings.BotToken);
        await _client.StartAsync();

    }
    public Task StopAsync()
    {
        return Task.CompletedTask;
    }


    private static Task LogAsync(LogMessage message)
    {
        Log.Information(message.ToString());
        return Task.CompletedTask;
    }

}
