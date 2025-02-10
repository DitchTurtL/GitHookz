using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GitHookz.Data.Bot.Modules;
using System.Reflection;

namespace GitHookz.Data.Bot;

public class InteractionHandler
{
    private readonly ILogger<InteractionHandler> _logger;
    private readonly IServiceProvider _services;
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;

    public InteractionHandler(ILogger<InteractionHandler> logger, IServiceProvider services, DiscordSocketClient client, InteractionService handler)
    {
        _logger = logger;
        _services = services;
        _client = client;
        _handler = handler;
    }

    public async Task InitializeAsync()
    {
        _handler.Log += LogAsync;
        _client.Ready += ReadyAsync;

        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        //await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _handler.AddModuleAsync<GitHookzModule>(_services);


        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;

        // Also process the result of the command execution.
        _handler.InteractionExecuted += HandleInteractionExecute;
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, interaction);

            // Execute the incoming command.
            var result = await _handler.ExecuteCommandAsync(context, _services);

            // Due to async nature of InteractionFramework, the result here may always be success.
            // That's why we also need to handle the InteractionExecuted event.
            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    default:
                        break;
                }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }

    private Task HandleInteractionExecute(ICommandInfo commandInfo, IInteractionContext context, Discord.Interactions.IResult result)
    {
        if (!result.IsSuccess)
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                default:
                    break;
            }

        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        // Register the commands globally.
        // alternatively you can use _handler.RegisterCommandsGloballyAsync() to register commands to a specific guild.
        var guildId = Environment.GetEnvironmentVariable("GUILD_ID");
        if (string.IsNullOrEmpty(guildId))
        {
            _logger.LogError("Guild ID is not set.");
            return;
        }

        if (ulong.TryParse(guildId, out ulong gid))
            await _handler.RegisterCommandsToGuildAsync(gid);
        else
            await _handler.RegisterCommandsGloballyAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.LogInformation(log.ToString());
        return Task.CompletedTask;
    }
}
