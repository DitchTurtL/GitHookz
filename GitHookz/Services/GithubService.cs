
using GitHookz.Data;
using GitHookz.Data.Bot.ResponseArguments;
using GitHookz.Data.Github;
using Octokit;
using System.Text.Json;

namespace GitHookz.Services;

public class GithubService : IGithubService
{
    private readonly ILogger<GithubService> _logger;
    private readonly IDatabaseService _databaseService;
    private readonly IBotService _botService;

    public GithubService(ILogger<GithubService> logger, IDatabaseService databaseService, IBotService botService)
    {
        _logger = logger;
        _databaseService = databaseService;
        _botService = botService;
    }

    public async Task<GithubProjectDetails> GetRepositoryDetails(string repositoryUrl)
    {
        var identifiers = GetRepositoryIdentifiersFromUrl(repositoryUrl);
        var owner = identifiers.Item1;
        var repo = identifiers.Item2;

        var client = new GitHubClient(new ProductHeaderValue("GitHookz"));
        
        var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(githubToken))
        {
            throw new Exception("GITHUB_TOKEN environment variable is not set");
        }

        var tokenAuth = new Credentials(githubToken);
        client.Credentials = tokenAuth;

        // Get repository info
        var repository = await client.Repository.Get(owner, repo);

        return new GithubProjectDetails()
        {
            Name = repository.Name,
            Description = repository.Description,
            StargazersCount = repository.StargazersCount,
            ForksCount = repository.ForksCount,
            Language = repository.Language,
            CreatedAt = repository.CreatedAt,
            UpdatedAt = repository.UpdatedAt
        };
    }

    private Tuple<string, string> GetRepositoryIdentifiersFromUrl(string repositoryUrl)
    {
        //https://github.com/DitchTurtL/GitHookz
        var parts = repositoryUrl.Split('/');

        var owner = parts[3];
        var repo = parts[4];

        return new Tuple<string, string>(owner, repo);
    }

    public async Task HandleWebhook(JsonDocument payload, HttpContext context)
    {
        // GitHub sends a secret signature in the headers (optional)
        string? signature = context.Request.Headers["X-Hub-Signature-256"];

        // Log the event type
        string? eventType = context.Request.Headers["X-GitHub-Event"];

        if (string.IsNullOrEmpty(eventType))
        {
            _logger.LogWarning("No event type found in headers");
            return;
        }

        await HandleWebHookOfType(eventType, payload);
    }

    private Task HandleWebHookOfType(string type, JsonDocument payload)
    {
        _logger.LogInformation($"Received GitHub event: {type}");

        return type switch {
            "ping" => HandlePingEvent(payload),
            "push" => HandlePushEvent(payload),
            "pull_request" => HandlePullRequestEvent(payload),
            "pull_request_review_comment" => HandlePullRequestReviewCommentEvent(payload),
            "issues" => HandleIssuesEvent(payload),
            "issue_comment" => HandleIssueCommentEvent(payload),
            "release" => HandleReleaseEvent(payload),
            "fork" => HandleForkEvent(payload),
            "watch" => HandleStarEvent(payload),
            _ => Task.CompletedTask
        };
    }

    private RepoDetails? GetRepoDetails(JsonDocument payload)
    {
        // Get repository node
        var repoNode = payload.RootElement.GetProperty("repository");
        if (repoNode.ValueKind == JsonValueKind.Undefined)
        {
            _logger.LogWarning("No repository node found in payload");
            return null;
        }

        // Get name and url values
        var repoName = repoNode.GetProperty("name").GetString();
        var repoUrl = repoNode.GetProperty("html_url").GetString();
        if (string.IsNullOrEmpty(repoName) || string.IsNullOrEmpty(repoUrl))
        {
            _logger.LogWarning("No repository name or url found in payload");
            return null;
        }

        return new RepoDetails(repoName, repoUrl);
    }

    private SenderDetails? GetSenderDetails(JsonDocument payload)
    {
        // Get sender node
        var senderNode = payload.RootElement.GetProperty("sender");
        if (senderNode.ValueKind == JsonValueKind.Undefined)
        {
            _logger.LogWarning("No sender node found in payload");
            return null;
        }

        // Get name and url values 
        var senderUsername = senderNode.GetProperty("login").GetString();
        var senderProfileUrl = senderNode.GetProperty("html_url").GetString();
        var senderAvatarUrl = senderNode.GetProperty("avatar_url").GetString();
        if (string.IsNullOrEmpty(senderUsername))
        {
            _logger.LogWarning("No sender username found in payload");
            return null;
        }

        return new SenderDetails(senderUsername, senderAvatarUrl, senderProfileUrl);
    }

    private ProjectData? GetProjectForUrl(string repositoryName, string repositoryUrl)
    {
        // Get repo reference from db
        var project = _databaseService.GetProjectByRepositoryUrl(repositoryUrl);
        if (project == null)
        {
            _logger.LogWarning("No project found for {name} ({url})", repositoryName, repositoryUrl);
            return null;
        }
        if (string.IsNullOrEmpty(project.ChannelId))
        {
            _logger.LogWarning("No channel id found for {name} ({url})", repositoryName, repositoryUrl);
            return null;
        }

        return project;
    }

    /// <summary>
    /// Handles the incoming ping event from GitHub
    /// </summary>
    private async Task HandlePingEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;
        
        _logger.LogInformation("Ping event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        
        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        var pingArgs = new PingReponseArguments(repoDetails);
        await _botService.SendPingMessage(project.ChannelId!, pingArgs);
    }

    /// <summary>
    /// Handles the incoming push event from GitHub
    /// </summary>
    private async Task HandlePushEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;

        _logger.LogInformation("Push event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);

        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        var senderDetails = GetSenderDetails(payload);
        if (senderDetails == null) return;

        var commitsList = payload.RootElement.GetProperty("commits").EnumerateArray().ToList();
        var lastCommit = commitsList.LastOrDefault();
        if (lastCommit.ValueKind == JsonValueKind.Undefined)
        {
            _logger.LogWarning("No commits found in payload");
            return;
        }

        var branchName = payload.RootElement.GetProperty("ref").GetString();
        var commitMessage = lastCommit.GetProperty("message").GetString();
        var addedCount = lastCommit.GetProperty("added").GetArrayLength();
        var removedCount = lastCommit.GetProperty("removed").GetArrayLength();
        var modifiedCount = lastCommit.GetProperty("modified").GetArrayLength();

        if (string.IsNullOrEmpty(commitMessage) || string.IsNullOrEmpty(branchName))
        {
            _logger.LogWarning("Missing branch name or commit message in payload");
            return;
        }

        branchName = branchName.Replace("refs/heads/", "");
        var pushArgs = new PushResponseArguments(repoDetails, senderDetails, branchName, commitMessage)
        {
            AddedCount = addedCount,
            RemovedCount = removedCount,
            ModifiedCount = modifiedCount
        };
        await _botService.SendPushMessage(project.ChannelId!, pushArgs);
    }

    private async Task HandlePullRequestEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;
        
        _logger.LogInformation("Pull request event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        
        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        // TODO: Gather pull request details


        var pullRequestArgs = new PullRequestResponseArguments(repoDetails);
        await _botService.SendPullRequestMessage(project.ChannelId!, pullRequestArgs);
    }

    private async Task HandlePullRequestReviewCommentEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;

        _logger.LogInformation("Pull request review comment event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        
        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        // TODO: Gather pull request review comment details

        var pullRequestReviewCommentArgs = new PullRequestReviewCommentResponseArguments(repoDetails);
        await _botService.SendPullRequestReviewCommentMessage(project.ChannelId!, pullRequestReviewCommentArgs);
    }

    private async Task HandleIssuesEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;

        _logger.LogInformation("Issues event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        
        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        // TODO: Gather issue details

        var issueArgs = new IssueResponseArguments(repoDetails);
        await _botService.SendIssueMessage(project.ChannelId!, issueArgs);
    }

    private async Task HandleIssueCommentEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;

        _logger.LogInformation("Issue comment event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        
        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        // TODO: Gather issue comment details

        var issueCommentArgs = new IssueCommentResponseArguments(repoDetails);
        await _botService.SendIssueCommentMessage(project.ChannelId!, issueCommentArgs);
    }

    private async Task HandleReleaseEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;

        _logger.LogInformation("Release event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        
        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        // TODO: Gather release details

        var releaseArgs = new ReleaseResponseArguments(repoDetails);
        await _botService.SendReleaseMessage(project.ChannelId!, releaseArgs);
    }

    private async Task HandleForkEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;
        _logger.LogInformation("Fork event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);

        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        // TODO: Gather fork details

        var forkArgs = new ForkResponseArguments(repoDetails);
        await _botService.SendForkMessage(project.ChannelId!, forkArgs);
    }

    private async Task HandleStarEvent(JsonDocument payload)
    {
        var repoDetails = GetRepoDetails(payload);
        if (repoDetails == null) return;

        _logger.LogInformation("Star event received for {name} ({url})", repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        
        var project = GetProjectForUrl(repoDetails.RepositoryName, repoDetails.RepositoryUrl);
        if (project == null) return;

        // TODO: Gather star details

        var starArgs = new StarResponseArguments(repoDetails);
        await _botService.SendStarMessage(project.ChannelId!, starArgs);
    }
}
