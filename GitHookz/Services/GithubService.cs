
using GitHookz.Data;
using Octokit;

namespace GitHookz.Services;

public class GithubService : IGithubService
{
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

}
