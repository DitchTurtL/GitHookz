namespace GitHookz.Data;

public class ProjectData
{
    public int Id { get; set; }
    public string? RepositoryName { get; set; }
    public string? RepositoryUrl { get; set; }
    public string? OwnerId { get; set; }
    public string? ChannelId { get; set; }
    public string? ChannelName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public bool IsPublished { get; set; }

    public bool CanSendPing { get; set; }
    public bool CanSendIssue { get; set; }
    public bool CanSendIssueComment { get; set; }
    public bool CanSendPullRequest { get; set; }
    public bool CanSendPush { get; set; }
    public bool CanSendPull { get; set; }
    public bool CanSendPullRequestReview { get; set; }
    public bool CanSendRelease { get; set; }
    public bool CanSendFork { get; set; }
    public bool CanSendStar { get; set; }
    public bool CanSendWatch { get; set; }
}
