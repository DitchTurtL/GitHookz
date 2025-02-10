using Discord.Interactions;
using GitHookz.Services;

namespace GitHookz.Data.Bot.Modules;

public class GitHookzModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<GitHookzModule> _logger;
    private readonly IAuthenticationService _authenticationService;

    public InteractionService? Commands { get; set; }

    private InteractionHandler _handler;

    // Constructor injection is also a valid way to access the dependencies
    public GitHookzModule(ILogger<GitHookzModule> logger, InteractionHandler handler, IAuthenticationService authenticationService)
    {
        _logger = logger;
        _handler = handler;
        _authenticationService = authenticationService;
    }

    [SlashCommand("webhook", "Connects a GitHub webhook to a Discord channel for realtime updates")]
    public async Task AddHookAsync([Summary(description: "GitHub Repo URL")] string repositoryUrl)
    {
        // Send the user a link to configure the repo/webhook
        _logger.LogInformation("AddHook command received with url: {url}", repositoryUrl);

        var sessionData = new SessionData
        {
            UserId = Context.User.Id.ToString(),
            UserName = Context.User.Username,
            ChannelId = Context.Channel.Id.ToString(),
            ChannelName = Context.Channel.Name,
            RepositoryName = StringHelper.GetRepoNameFromUrl(repositoryUrl),
            RepositoryUrl = repositoryUrl
        };

        var newSessionId = _authenticationService.CreateSession(sessionData);

        if (string.IsNullOrEmpty(newSessionId))
        {
            await RespondAsync("Failed to create session.");
        }
        else
        {
            var sessionUrl = _authenticationService.GetSessionUrl(newSessionId);
            await RespondAsync($"Configure your project: {sessionUrl}");
        }
    }

    [SlashCommand("githookz", "Requests a link to manage your webhooks")]
    public async Task GitHookzAsync()
    {
        // TODO: Send an authenticated link to the user's DMs
        _logger.LogInformation("GitHookz command received.");
        await RespondAsync("Pong!");
    }

}
