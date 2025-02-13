
using GitHookz.Data;
using Octokit;
using System.Text.Json;

namespace GitHookz.Services;

public class GithubService : IGithubService
{
    private readonly ILogger<GithubService> _logger;

    public GithubService(ILogger<GithubService> logger)
    {
        _logger = logger;
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

        var json = payload.RootElement.ToString();

        await HandleWebHookOfType(eventType, payload);
    }

    private Task HandleWebHookOfType(string type, JsonDocument payload)
    {
        _logger.LogInformation($"Received GitHub event: {type}");

        return type switch {
            "ping" => HandlePingEvent(payload),
            "push" => HandlePushEvent(payload),
            //"pull_request" => HandlePullRequestEvent(payload),
            //"issues" => HandleIssuesEvent(payload),
            //"issue_comment" => HandleIssueCommentEvent(payload),
            //"release" => HandleReleaseEvent(payload),
            //"fork" => HandleForkEvent(payload),
            //"watch" => HandleStarEvent(payload),
            _ => Task.CompletedTask
        };
    }

    private async Task HandlePingEvent(JsonDocument payload)
    {
        throw new NotImplementedException();
    }

    private async Task HandlePushEvent(JsonDocument payload)
    {
        var json = payload.RootElement.ToString();
        throw new NotImplementedException();
    }

    private async Task HandlePullRequestEvent(JsonDocument payload)
    {
        throw new NotImplementedException();
    }

    private async Task HandleIssuesEvent(JsonDocument payload)
    {
        throw new NotImplementedException();
    }

    private async Task HandleIssueCommentEvent(JsonDocument payload)
    {
        throw new NotImplementedException();
    }

    private async Task HandleReleaseEvent(JsonDocument payload)
    {
        throw new NotImplementedException();
    }

    private async Task HandleForkEvent(JsonDocument payload)
    {
        throw new NotImplementedException();
    }
    private async Task HandleStarEvent(JsonDocument payload)
    {
        throw new NotImplementedException();
    }



    /*
        Events to handle:
        - Push
        - Pull Request
        - Issues
        - Comments
        - Releases
        - Forks
        - Stars



    */
}
