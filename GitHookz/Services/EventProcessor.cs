using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;

namespace GitHookz.Services;

public class EventProcessor : WebhookEventProcessor
{
    private readonly IDatabaseService _databaseService;

    public EventProcessor(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    protected override Task ProcessCreateWebhookAsync(WebhookHeaders headers, CreateEvent createEvent)
    {
        var repoName = createEvent.Repository?.FullName ?? "Unknown";

        return base.ProcessCreateWebhookAsync(headers, createEvent);
    }

    protected override Task ProcessDeleteWebhookAsync(WebhookHeaders headers, DeleteEvent deleteEvent)
    {
        return base.ProcessDeleteWebhookAsync(headers, deleteEvent);
    }

    protected override Task ProcessPullRequestWebhookAsync(WebhookHeaders headers, PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        return base.ProcessPullRequestWebhookAsync(headers, pullRequestEvent, action);
    }

}
