using Discord.Interactions;
using Discord;
using GitHookz.Data.Models;
using GitHookz.Services;
using GitHookz.Data;

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

    [SlashCommand("webhook", "Create a new webhook at this location for your project repo.")]
    public async Task AddWebhook([Summary(description: "Repo URL")] string repoUrl)
    {
        RecipientType type;
        if (Context.Channel is IThreadChannel)
            type = RecipientType.Thread;
        else
            type = RecipientType.Channel;

        var repoDetails = RepoDetails.GetRepoDetails(repoUrl);
        var repoFullName = $"{repoDetails.Owner}/{repoDetails.Repo}";

        var data = new WebHookData
        {
            Type = type,
            GuildId = (long)Context.Guild.Id,
            RecipientId = (long)Context.Channel.Id,
            RecipientName = Context.Channel.Name,
            RepoFullname = repoFullName,
            AddedBy = Context.User.Username,
            AddedAt = DateTime.Now,
            AddedById = (long)Context.User.Id
        };

        var success = _databaseService.AddWebHookData(data);

        var webhookUrl = $"{_config["external_url"]}{_config["webhook_endpoint"]}";
        await RespondAsync($"Your repo was added. Use this URL for your webhook: {webhookUrl}");
    }
}
