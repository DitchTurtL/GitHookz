using GitHookz.Data;
using System.Text.Json;

namespace GitHookz.Services;

public interface IGithubService
{
    Task<GithubProjectDetails?> GetRepositoryDetails(string repositoryUrl);
    Task HandleWebhook(JsonDocument payload, HttpContext context);
}