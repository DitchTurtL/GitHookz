using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.PullRequest;

namespace GitHookz.Services
{
    public class EventProcessor : WebhookEventProcessor
    {
        protected override Task ProcessCreateWebhookAsync(WebhookHeaders headers, CreateEvent createEvent)
        {
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
}
