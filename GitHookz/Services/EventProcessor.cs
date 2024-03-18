using Discord;
using GitHookz.Data;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;
using Serilog;
using System.Threading.Channels;

namespace GitHookz.Services;

public class EventProcessor : WebhookEventProcessor
{
    private readonly IDatabaseService _databaseService;
    private readonly IInteractionHandler _interactionHandler;

    public EventProcessor(IDatabaseService databaseService, IInteractionHandler interactionHandler)
    {
        _databaseService = databaseService;
        _interactionHandler = interactionHandler;
    }

    protected override Task ProcessCreateWebhookAsync(WebhookHeaders headers, CreateEvent createEvent)
    {
        var repoName = createEvent.Repository?.FullName ?? "Unknown";
        var targets = _databaseService.GetWebHookDataByRepoFullName(repoName);


        return base.ProcessCreateWebhookAsync(headers, createEvent);
    }

    protected override Task ProcessDeleteWebhookAsync(WebhookHeaders headers, DeleteEvent deleteEvent)
    {
        return base.ProcessDeleteWebhookAsync(headers, deleteEvent);
    }

    protected override Task ProcessPushWebhookAsync(WebhookHeaders headers, PushEvent pushEvent)
    {
        var repoName = pushEvent.Repository?.FullName ?? "Unknown";
        var repoUrl = pushEvent.Repository?.HtmlUrl ?? "Unknown";
        var authorName = pushEvent.Pusher?.Name ?? "Unknown";
        var targets = _databaseService.GetWebHookDataByRepoFullName(repoName);
        var commits = pushEvent.Commits;
        var actionText = $"Pushed {commits.Count()} commits to {repoName}";

        var embed = new EmbedBuilder()
            .WithTitle(actionText)
            .WithUrl(repoUrl)
            .WithAuthor(authorName)
            .WithFooter(StringConstants.APP_NAME)
            .WithColor(Discord.Color.Blue);

        foreach (var commit in commits)
        {
            var commitId = commit.Id[0..5];
            var commitUrl = commit.Url;
            var commitMessage = commit.Message;

            var newField = new EmbedFieldBuilder
            {
                Name = commitId,
                Value = $"[{commitId}]({commitUrl}) {commitMessage}"
            };

            embed.AddField(newField);
        }

        foreach (var target in targets)
            _interactionHandler.SendPushMessageAsync(target.RecipientId, embed.Build());

        return base.ProcessPushWebhookAsync(headers, pushEvent);
    }

    /// <summary>
    /// Process a Pull Request Event
    /// </summary>
    protected override Task ProcessPullRequestWebhookAsync(WebhookHeaders headers, PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        // Get the repo's FullName. This is how we will identify the targets
        var repoName = pullRequestEvent.Repository?.FullName ?? "Unknown";

        // Get all target channels/threads for this repo to be broadcast to
        var targets = _databaseService.GetWebHookDataByRepoFullName(repoName);

        // Action text for what the user did
        var actionText = StringConstants.ToTitleCase(pullRequestEvent.Action ?? "Interacted with") + " a Pull Request";

        Log.Information($"Processing Pull Request Event: {actionText} on {repoName}");

        foreach (var target in targets)
            _interactionHandler.SendMessageAsync(
                target.RecipientId,
                pullRequestEvent.Repository?.Name ?? "Unknown Repo",
                pullRequestEvent.Sender?.Login ?? "Unknown User",
                actionText,
                pullRequestEvent.PullRequest?.Title ?? "No Title",
                pullRequestEvent.Sender?.AvatarUrl ?? "",
                pullRequestEvent.PullRequest?.HtmlUrl ?? ""
                );

        return base.ProcessPullRequestWebhookAsync(headers, pullRequestEvent, action);
    }
}