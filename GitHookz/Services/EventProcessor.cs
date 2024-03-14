using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;

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

    protected override Task ProcessPullRequestWebhookAsync(WebhookHeaders headers, PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        var repoName = pullRequestEvent.Repository?.FullName ?? "Unknown";
        var targets = _databaseService.GetWebHookDataByRepoFullName(repoName);
        foreach (var target in targets)
            _interactionHandler.SendMessageAsync(target.RecipientId);


        return base.ProcessPullRequestWebhookAsync(headers, pullRequestEvent, action);
    }

}
