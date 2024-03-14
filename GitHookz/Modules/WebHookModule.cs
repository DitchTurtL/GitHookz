using Discord.Interactions;
using Discord;
using GitHookz.Data.Models;
using GitHookz.Services;

namespace GitHookz.Modules;

public class WebHookModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IConfiguration _config;
    private readonly IDatabaseService _databaseService;

    public WebHookModule(IConfiguration config, IDatabaseService databaseService)
    {
        _config = config;
        _databaseService = databaseService;
    }

    [SlashCommand("add_webhook", "Create a new webhook at this location for your project repo.")]
    public async Task AddWebhook([Summary(description: "Repo URL")] string repoUrl)
    {
        string type = string.Empty;
        if (Context.Channel is IThreadChannel)
        {
            type = "T";
        }
        else
        {
            type = "C";
        }

        var channelId = Context.Channel.Id.ToString();
        var repoDetails = RepoDetails.GetRepoDetails(repoUrl);
        var repoFullName = $"{repoDetails.Owner}/{repoDetails.Repo}";

        var data = new WebHookData(type, channelId, repoFullName);
        var success = _databaseService.AddWebHookData(data);

        // TODO: Keep track of the repo details so when we get a webhook we can send it to the right place

        await RespondAsync($"Your repo was added. Use this URL for your webhook: {_config["external_url"]}{_config["webhook_endpoint"]}");
    }

    public async Task Echo(string echo, [Summary(description: "mention the user")] bool mention = false)
    => await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));
}
