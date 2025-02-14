using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class PullRequestResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public PullRequestResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
