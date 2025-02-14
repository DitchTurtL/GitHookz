using GitHookz.Data.Bot.ResponseArguments;

namespace GitHookz.Services;

public interface IBotService
{
    Task StartBotAsync();

    Task SendPingMessage(string channelId, PingReponseArguments arguments);
    Task SendPushMessage(string channelId, PushResponseArguments arguments);
    Task SendPullRequestMessage(string channelId, PullRequestResponseArguments arguments);
    Task SendPullRequestReviewCommentMessage(string channelId, PullRequestReviewCommentResponseArguments arguments);
    Task SendIssueMessage(string channelId, IssueResponseArguments arguments);
    Task SendIssueCommentMessage(string channelId, IssueCommentResponseArguments arguments);
    Task SendReleaseMessage(string channelId, ReleaseResponseArguments arguments);
    Task SendForkMessage(string channelId, ForkResponseArguments arguments);
    Task SendStarMessage(string channelId, StarResponseArguments arguments);
}