using GitHookz.Data.Github;

namespace GitHookz.Data.Bot.ResponseArguments;

public class IssueResponseArguments
{
    public RepoDetails RepositoryDetails { get; set; }

    public IssueResponseArguments(RepoDetails repositoryDetails)
    {
        RepositoryDetails = repositoryDetails;
    }
}
