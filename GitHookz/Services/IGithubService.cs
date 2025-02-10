using GitHookz.Data;

namespace GitHookz.Services;

public interface IGithubService
{
    Task<GithubProjectDetails?> GetRepositoryDetails(string repositoryUrl);
}